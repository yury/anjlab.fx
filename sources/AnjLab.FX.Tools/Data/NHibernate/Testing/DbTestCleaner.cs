using System;
using System.Collections.Generic;
using System.Text;
using AnjLab.FX.Tools.Testing;
using NHibernate;

namespace AnjLab.FX.Tools.Data.NHibernate.Testing
{
    public class DbTestCleaner : ITestCleaner
    {
        private string _sessionFactoryConfigPath;
        private TestInterceptor _interceptor = new TestInterceptor();
        
        public DbTestCleaner(string sessionFactoryConfigPath)
        {
            _sessionFactoryConfigPath = sessionFactoryConfigPath;
        }
        
        #region ITestCleaner Members

        public void OnSetup()
        {
            NHibernateSessionManager.Instance.BeginTransactionOn(_sessionFactoryConfigPath);
            if (!NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryConfigPath).IsOpen)
                NHibernateSessionManager.Instance.RegisterInterceptorOn(_sessionFactoryConfigPath, _interceptor);
        }

        public void ClearCache()
        {
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryConfigPath);
            session.Clear();
        }

        public void OnTearDown()
        {
            NHibernateSessionManager.Instance.RollbackTransactionOn(_sessionFactoryConfigPath);
            ISession session = NHibernateSessionManager.Instance.GetSessionFrom(_sessionFactoryConfigPath);
            session.Clear();
            _interceptor.SavedObjects.Reverse();
            foreach (object entity in _interceptor.SavedObjects)
            {
                //try
                //{
                //session.Update(entity);
                session.Delete(entity);

                //}
                //catch (Exception e)
                //{
                //}
            }

            session.Flush();
        }

        #endregion
    }
}
