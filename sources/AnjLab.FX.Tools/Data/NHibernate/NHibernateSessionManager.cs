using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Messaging;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;
using AnjLab.FX.System;
using System.Web;

namespace AnjLab.FX.Tools.Data.NHibernate
{
    /// <summary>
    /// Handles creation and management of sessions and transactions.  It is a singleton because 
    /// building the initial session factory is very expensive. Inspiration for this class came 
    /// from Chapter 8 of Hibernate in Action by Bauer and King.  Although it is a sealed singleton
    /// you can use TypeMock (http://www.typemock.com) for more flexible testing.
    /// </summary>
    public sealed class NHibernateSessionManager
    {
        #region Thread-safe, lazy Singleton
        private static object _lockObj = new object();

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static NHibernateSessionManager Instance {
            get {
                return Nested.NHibernateSessionManager;
            }
        }

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        private NHibernateSessionManager() { }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly NHibernateSessionManager NHibernateSessionManager =
                new NHibernateSessionManager();
        }

        #endregion

        /// <summary>
        /// This method attempts to find a session factory stored in <see cref="sessionFactories" />
        /// via its name; if it can't be found it creates a new one and adds it the hashtable.
        /// </summary>
        /// <param name="sessionFactoryConfigPath">Path location of the factory config</param>
        private ISessionFactory GetSessionFactoryFor(string sessionFactoryConfigPath) {
            Guard.ArgumentNotNullNorEmpty(sessionFactoryConfigPath, "sessionFactoryConfigPath may not be null nor empty");

            //  Attempt to retrieve a stored SessionFactory from the hashtable.
            ISessionFactory sessionFactory = (ISessionFactory) sessionFactories[sessionFactoryConfigPath];

            //  Failed to find a matching SessionFactory so make a new one.
            if (sessionFactory == null) {
                if (!File.Exists(sessionFactoryConfigPath))
                {
                    sessionFactoryConfigPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                                               sessionFactoryConfigPath);
                }
                Guard.IsTrue(File.Exists(sessionFactoryConfigPath),
                    "The config file at '" + sessionFactoryConfigPath + "' could not be found");
                
                Configuration cfg = new Configuration();
                cfg.Configure(sessionFactoryConfigPath);

                //  Now that we have our Configuration object, create a new SessionFactory
                sessionFactory = cfg.BuildSessionFactory();

                if (sessionFactory == null) {
                    throw new InvalidOperationException("cfg.BuildSessionFactory() returned null.");
                }

                lock (_lockObj)
                {
                    if (!sessionFactories.ContainsKey(sessionFactoryConfigPath))
                        sessionFactories.Add(sessionFactoryConfigPath, sessionFactory);
                }
            }

            return sessionFactory;
        }

        /// <summary>
        /// Allows you to register an interceptor on a new session.  This may not be called if there is already
        /// an open session attached to the HttpContext.  If you have an interceptor to be used, modify
        /// the HttpModule to call this before calling BeginTransaction().
        /// </summary>
        public void RegisterInterceptorOn(string sessionFactoryConfigPath, IInterceptor interceptor) {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session != null && session.IsOpen) {
                throw new CacheException("You cannot register an interceptor once a session has already been opened");
            }

            GetSessionFrom(sessionFactoryConfigPath, interceptor);
        }

        public ISession GetSessionFrom(string sessionFactoryConfigPath) {
            return GetSessionFrom(sessionFactoryConfigPath, null);
        }

        /// <summary>
        /// Gets a session with or without an interceptor.  This method is not called directly; instead,
        /// it gets invoked from other public methods.
        /// </summary>
        private ISession GetSessionFrom(string sessionFactoryConfigPath, IInterceptor interceptor) {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session == null) {
                if (interceptor != null) {
                    session = GetSessionFactoryFor(sessionFactoryConfigPath).OpenSession(interceptor);
                }
                else {
                    session = GetSessionFactoryFor(sessionFactoryConfigPath).OpenSession();
                }

                ContextSessions[sessionFactoryConfigPath] = session;
            }

            Guard.NotNull(session);

            return session;
        }

        /// <summary>
        /// Flushes anything left in the session and closes the connection.
        /// </summary>
        public void CloseSessionOn(string sessionFactoryConfigPath) {
            ISession session = (ISession)ContextSessions[sessionFactoryConfigPath];

            if (session != null && session.IsOpen) {
                session.Flush();
                session.Close();
            }

            ContextSessions.Remove(sessionFactoryConfigPath);
        }

        public ITransaction BeginTransactionOn(string sessionFactoryConfigPath) {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            if (transaction == null) {
                transaction = GetSessionFrom(sessionFactoryConfigPath).BeginTransaction();
                ContextTransactions.Add(sessionFactoryConfigPath, transaction);
            }

            return transaction;
        }

        public void CommitTransactionOn(string sessionFactoryConfigPath) {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            try {
                if (HasOpenTransactionOn(sessionFactoryConfigPath)) {
                    transaction.Commit();
                    ContextTransactions.Remove(sessionFactoryConfigPath);
                }
            }
            catch (HibernateException) {
                RollbackTransactionOn(sessionFactoryConfigPath);
                throw;
            }
        }

        public bool HasOpenTransactionOn(string sessionFactoryConfigPath) {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public void RollbackTransactionOn(string sessionFactoryConfigPath) {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryConfigPath];

            try {
                if (HasOpenTransactionOn(sessionFactoryConfigPath)) {
                    transaction.Rollback();
                }

                ContextTransactions.Remove(sessionFactoryConfigPath);
            }
            finally {
                CloseSessionOn(sessionFactoryConfigPath);
            }
        }

        /// <summary>
        /// Since multiple databases may be in use, there may be one transaction per database 
        /// persisted at any one time.  The easiest way to store them is via a hashtable
        /// with the key being tied to session factory.  If within a web context, this uses
        /// <see cref="HttpContext" /> instead of the WinForms specific <see cref="CallContext" />.  
        /// Discussion concerning this found at http://forum.springframework.net/showthread.php?t=572
        /// </summary>
        private Hashtable ContextTransactions {
            get {
                if (IsInWebContext()) { 
                    if (HttpContext.Current.Items[TRANSACTION_KEY] == null)
                        HttpContext.Current.Items[TRANSACTION_KEY] = new Hashtable();

                    return (Hashtable)HttpContext.Current.Items[TRANSACTION_KEY];
                }
                else {
                    if (CallContext.GetData(TRANSACTION_KEY) == null)
                        CallContext.SetData(TRANSACTION_KEY, new Hashtable());

                    return (Hashtable)CallContext.GetData(TRANSACTION_KEY);
                }
            }
        }

        /// <summary>
        /// Since multiple databases may be in use, there may be one session per database 
        /// persisted at any one time.  The easiest way to store them is via a hashtable
        /// with the key being tied to session factory.  If within a web context, this uses
        /// <see cref="HttpContext" /> instead of the WinForms specific <see cref="CallContext" />.  
        /// Discussion concerning this found at http://forum.springframework.net/showthread.php?t=572
        /// </summary>
        private Hashtable ContextSessions {
            get {
                if (IsInWebContext()) {
                    if (HttpContext.Current.Items[SESSION_KEY] == null)
                        HttpContext.Current.Items[SESSION_KEY] = new Hashtable();

                    return (Hashtable)HttpContext.Current.Items[SESSION_KEY];
                }
                else {
                    if (CallContext.GetData(SESSION_KEY) == null)
                        CallContext.SetData(SESSION_KEY, new Hashtable());

                    return (Hashtable)CallContext.GetData(SESSION_KEY);
                }
            }
        }

        private bool IsInWebContext() {
            return HttpContext.Current != null;
        }

        private Hashtable sessionFactories = new Hashtable();
        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTIONS";
        private const string SESSION_KEY = "CONTEXT_SESSIONS";
    }
}
