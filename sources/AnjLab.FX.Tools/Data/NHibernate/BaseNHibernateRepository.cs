using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using AnjLab.FX.System;
using NHibernate;
using NHibernate.Expression;
using AnjLab.FX.Data;

namespace AnjLab.FX.Tools.Data.NHibernate
{
    public class BaseNHibernateRepository<T> : IRepository<T>
    {
        /// <param name="sessionFactoryConfigPath">Fully qualified path of the session factory's config file</param>
        public BaseNHibernateRepository(string sessionFactoryConfigPath) {
            if (!Path.IsPathRooted(sessionFactoryConfigPath))
                if (HttpContext.Current != null)
                    sessionFactoryConfigPath = HttpContext.Current.ApplicationInstance.Server.MapPath(sessionFactoryConfigPath);

            Guard.ArgumentNotNullNorEmpty(sessionFactoryConfigPath, "sessionFactoryConfigPath may not be null nor empty");

            SessionFactoryConfigPath = sessionFactoryConfigPath;
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public T GetByID(object id)
        {
            return GetById(id, false);
        }

        /// <summary>
        /// Loads an instance of type T from the DB based on its ID.
        /// </summary>
        public T GetById(object id, bool shouldLock) {
            try {
                T entity;

                if (shouldLock) {
                    entity = (T) NHibernateSession.Load(persitentType, id, LockMode.Upgrade);
                } else {
                    entity = (T) NHibernateSession.Load(persitentType, id);
                }

                return entity;
            } finally {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        public T TryGetById(object id)
        {
            try
            {
                return GetByID(id);
            }
            catch (ObjectNotFoundException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Loads every instance of the requested type with no filtering.
        /// </summary>
        public IList<T> GetAll() {
            try {
                return GetByCriteria();
            } finally {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        public IList<T> GetAll(string orderByField, bool ascending)
        {
            try {
                ICriteria criteria = NHibernateSession.CreateCriteria(persitentType).SetCacheable(true);
                criteria.AddOrder(new Order(orderByField, ascending));
                return criteria.List<T>() as List<T>;
            }
            finally
            {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            criteria.SetProjection(Projections.Count(persitentType.GetProperties()[0].Name));
            return criteria.UniqueResult<int>();
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="orderByField">The order by field.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        /// <returns></returns>
        public IList<T> GetPage(int startIndex, int count, string orderByField, bool ascending)
        {
            try {
                ICriteria criteria = NHibernateSession.CreateCriteria(persitentType).SetCacheable(true);
                criteria.AddOrder(new Order(orderByField, ascending));
                criteria.SetFirstResult(startIndex);
                criteria.SetMaxResults(count);

                return criteria.List<T>() as List<T>;
            } finally {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }


        public IList<T> GetFirst(int count)
        {
            return GetByCriteria(count);
        }

        public IList<T> GetLast(string dateTimeProperty, DateTime dateTime, bool ascending)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            criteria.AddOrder(new Order(dateTimeProperty, ascending));
            criteria.Add(Expression.Ge(dateTimeProperty, dateTime));

            return criteria.List<T>() as List<T>;
        }

        public IList<T> GetLastIn<TIn>(string dateTimeProperty, DateTime dateTime, bool ascending, string inProperty,
                                  params TIn[] inValues)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            criteria.AddOrder(new Order(dateTimeProperty, ascending));
            criteria.Add(Expression.In(inProperty, inValues));
            if(dateTime != DateTime.MaxValue)
                criteria.Add(Expression.Ge(dateTimeProperty, dateTime));
            else
            {
                //select only one last value
                criteria.SetMaxResults(1);
            }

            return criteria.List<T>() as List<T>;
        }

        public IList<T> GetBeforeIn<TIn>(int count, string dateTimeProperty, DateTime dateTime, bool ascending, string inProperty,
                                  params TIn[] inValues)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            criteria.AddOrder(new Order(dateTimeProperty, ascending));
            criteria.Add(Expression.In(inProperty, inValues));
            criteria.Add(Expression.Le(dateTimeProperty, dateTime));
            if (count > 0)
                criteria.SetMaxResults(count);
            return criteria.List<T>() as List<T>;
        }

        public IList<T> GetBetweenIn<TIn>(string dateTimeProperty, DateTime beginTime, DateTime endTime, bool ascending, string inProperty,
                          params TIn[] inValues)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            criteria.AddOrder(new Order(dateTimeProperty, ascending));
            criteria.Add(Expression.In(inProperty, inValues));
            criteria.Add(Expression.Between(dateTimeProperty, beginTime, endTime));

            return criteria.List<T>() as List<T>;
        }

        /// <summary>
        /// Loads every instance of the requested type using the supplied <see cref="ICriterion" />.
        /// If no <see cref="ICriterion" /> is supplied, this behaves like <see cref="GetAll" />.
        /// </summary>
        public IList<T> GetByCriteria(params ICriterion[] criterion)
        {
            return GetByCriteria(0, criterion);
        }

        public IList<T> GetByCriteria(int maxCount, params ICriterion[] criterion)
        {
            try {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType).SetCacheable(true);
            if (maxCount > 0)
                criteria.SetMaxResults(maxCount);

            foreach (ICriterion criterium in criterion)
            {
                criteria.Add(criterium);
            }

            return criteria.List<T>() as List<T>;
            } finally
            {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        public IList<T> GetByProperty(string propertyName, object value)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType).SetCacheable(true);
            criteria.Add(Expression.Eq(propertyName, value));

            try
            {
                return criteria.List<T>();
            }
            catch(Exception)
            {
                return new List<T>();
            }
        }

        public IList<T> GetByExample(T exampleInstance, params string[] propertiesToExclude) {
            try {
                ICriteria criteria = NHibernateSession.CreateCriteria(persitentType).SetCacheable(true);
                Example example = Example.Create(exampleInstance);

                foreach (string propertyToExclude in propertiesToExclude) {
                    example.ExcludeProperty(propertyToExclude);
                }

                criteria.Add(example);

                return criteria.List<T>() as List<T>;
            } finally {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        /// <summary>
        /// Looks for a single instance using the example provided.
        /// </summary>
        /// <exception cref="NonUniqueResultException" />
        public T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude) {
            IList<T> foundList = GetByExample(exampleInstance, propertiesToExclude);

            if (foundList.Count > 1) {
                throw new NonUniqueResultException(foundList.Count);
            }

            if (foundList.Count > 0) {
                return foundList[0];
            }
            else {
                return default(T);
            }
        }

        protected IList<T> Find(string propertyName, object value, string orderByField, bool ascending, int? maxCount)
        {
            ICriteria criteria =
                NHibernateSession.CreateCriteria(persitentType).AddOrder(new Order(orderByField, ascending));

            if(!string.IsNullOrEmpty(propertyName))
                criteria.Add(Expression.Eq(propertyName, value));

            if (maxCount != null)
                criteria.SetMaxResults(maxCount.Value);

            return criteria.List<T>();
        }

        protected IList<T> ExecuteQuery<T>(string hql)
        {
            return NHibernateSession.CreateQuery(hql).List<T>();
        }

        /// <summary>
        /// For entities that have assigned ID's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/reference/en/html/mapping.html#mapping-declaration-id-assigned.
        /// </summary>
        public T Save(T entity) {
            try {
            NHibernateSession.Save(entity);
            return entity;
            } finally
            {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        /// <summary>
        /// For entities with automatatically generated IDs, such as identity, SaveOrUpdate may 
        /// be called when saving a new entity.  SaveOrUpdate can also be called to _update_ any 
        /// entity, even if its ID is assigned.
        /// </summary>
        public T SaveOrUpdate(T entity) {
            try {
                NHibernateSession.SaveOrUpdate(entity);
                return entity;
            } finally {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        public void Delete(T entity) {
            try {
                NHibernateSession.Delete(entity);
            } finally
            {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        public void Delete(string query)
        {
            try {
                NHibernateSession.Delete(query);
            } finally {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        public IList SqlQuery(string sqlQuery, Type returnType)
        {
            return NHibernateSession.CreateSQLQuery(sqlQuery).AddEntity(returnType).List();
        }

        public void DeleteAll()
        {
            try {
                NHibernateSession.Delete(String.Format("from {0}", typeof(T).Name));
            } finally {
                NHibernateSession.Flush();
                NHibernateSession.Clear();
            }
        }

        public IList Select(string query)
        {
            return NHibernateSession.CreateQuery(query).List();
        }

        /// <summary>
        /// Commits changes regardless of whether there's an open transaction or not
        /// </summary>
        public void CommitChanges() {
            if (NHibernateSessionManager.Instance.HasOpenTransactionOn(SessionFactoryConfigPath)) {
                NHibernateSessionManager.Instance.CommitTransactionOn(SessionFactoryConfigPath);
            }
            else {
                // If there's no transaction, just flush the changes
                NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryConfigPath).Flush();
            }
        }

        public void Refresh(T entity)
        {
            NHibernateSession.Refresh(entity);
        }

        /// <summary>
        /// Exposes the ISession used within the DAO.
        /// </summary>
        protected ISession NHibernateSession {
            get {
                return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryConfigPath);
            }
        }

        protected Type persitentType = typeof(T);
        protected readonly string SessionFactoryConfigPath;

        protected IDbCommand NewStoredProcedure(string name)
        {
            Guard.ArgumentNotNullNorEmpty("name", name);
 
            IDbCommand cmd = NHibernateSession.Connection.CreateCommand();
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;
            NHibernateSession.Transaction.Enlist(cmd);
            return cmd;
        }

        protected IDbCommand NewCommand(string text)
        {
            Guard.ArgumentNotNullNorEmpty("text", text);

            IDbCommand cmd = NHibernateSession.Connection.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = CommandType.Text;
            NHibernateSession.Transaction.Enlist(cmd);
            return cmd;
        }
    }
}
