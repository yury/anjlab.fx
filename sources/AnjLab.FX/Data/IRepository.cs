#if NET_3_5
using System;
using System.Collections;
using System.Collections.Generic;

namespace AnjLab.FX.Data
{
    public interface IRepository<T>
    {
        T GetByID(object id);
        IList<T> GetAll();
        IList<T> GetAll(string orderByField, bool ascending);
        IList<T> GetFirst(int count);
        IList<T> GetLast(string dateTimeProperty, DateTime dateTime, bool ascending);
        IList<T> GetLastIn<TIn>(string dateTimeProperty, DateTime dateTime, bool ascending, int count, string inProperty, params TIn[] inValues);
        IList<T> GetBeforeIn<TIn>(int count, string dateTimeProperty, DateTime dateTime, bool ascending, string inProperty, params TIn[] inValue);
        IList<T> GetBetweenIn<TIn>(string dateTimeProperty, DateTime beginTime, DateTime endTime, bool ascending, string inProperty, params TIn[] inValues);
        int GetCount();
        IList<T> GetPage(int startIndex, int count, string orderByField, bool ascending);
        IList<T> GetByProperty(string propertyName, object value);
        IList<T> GetByExample(T exampleInstance, params string[] propertiesToExclude);
        T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude);
        T Save(T entity);
        T SaveOrUpdate(T entity);
        void Delete(T entity);
        void Delete(string query);
        void DeleteAll();
        void CommitChanges();
        void Refresh(T entity);

        IList SqlQuery(string sqlQuery, Type returnType);
    }
}
#endif