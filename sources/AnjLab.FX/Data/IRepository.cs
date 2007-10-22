using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Data
{
    public interface IRepository<T>
    {
        T GetById(object id);
        IList<T> GetAll();
        IList<T> GetAll(string orderByField, bool ascending);
        IList<T> GetFirst(int count);
        int GetCount();
        IList<T> GetPage(int startIndex, int count, string orderByField, bool ascending);
        IList<T> GetByExample(T exampleInstance, params string[] propertiesToExclude);
        T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude);
        T Save(T entity);
        T SaveOrUpdate(T entity);
        void Delete(T entity);
        void CommitChanges();
        void Refresh(T entity);

    }
}
