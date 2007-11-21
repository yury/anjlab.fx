#if NET_3_5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Data
{
    public interface IRepository<T>
    {
        T GetByID(object id);
        IList<T> GetAll();
        IList<T> GetAll(string orderByField, bool ascending);
        IList<T> GetFirst(int count);
        IList<T> GetLast(string dateTimeProperty, DateTime dateTime, bool ascending);
        IList<T> GetLastIn(string dateTimeProperty, DateTime dateTime, bool ascending, string inProperty, params object[] inValues);
        int GetCount();
        IList<T> GetPage(int startIndex, int count, string orderByField, bool ascending);
        IList<T> GetByProperty(string propertyName, object value);
        IList<T> GetByExample(T exampleInstance, params string[] propertiesToExclude);
        T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude);
        T Save(T entity);
        T SaveOrUpdate(T entity);
        void Delete(T entity);
        void DeleteAll();
        void CommitChanges();
        void Refresh(T entity);
    }
}
#endif