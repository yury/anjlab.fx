using System;
using System.Configuration;
using System.Web;
using AnjLab.FX.System;

namespace AnjLab.FX.Tools.Data.NHibernate
{
    /// <summary>
    /// Implements the Open-Session-In-View pattern using <see cref="NHibernateSessionManager" />.
    /// Inspiration for this class came from Ed Courtenay at 
    /// http://sourceforge.net/forum/message.php?msg_id=2847509.
    /// </summary>
    public class NHibernateSessionModule : IHttpModule
    {
        public void Init(HttpApplication context) {
            context.BeginRequest += new EventHandler(BeginTransaction);
            context.EndRequest += new EventHandler(CommitAndCloseSession);
        }

        /// <summary>
        /// Opens a session within a transaction at the beginning of the HTTP request.  Note that 
        /// it ONLY begins transactions for those designated as being transactional.
        /// </summary>
        /// <param name="sender"></param>
        private void BeginTransaction(object sender, EventArgs e) {
            //OpenSessionInViewSection openSessionInViewSection = GetOpenSessionInViewSection();

            //foreach (SessionFactoryElement sessionFactorySettings in openSessionInViewSection.SessionFactories) {
            //    if (sessionFactorySettings.IsTransactional) {
            //        NHibernateSessionManager.Instance.BeginTransactionOn(sessionFactorySettings.FactoryConfigPath);
            //    }
            //}
        }

        /// <summary>
        /// Commits and closes the NHibernate session provided by the supplied <see cref="NHibernateSessionManager"/>.
        /// Assumes a transaction was begun at the beginning of the request; but a transaction or session does
        /// not *have* to be opened for this to operate successfully.
        /// </summary>
        private void CommitAndCloseSession(object sender, EventArgs e) {
            OpenSessionInViewSection openSessionInViewSection = GetOpenSessionInViewSection();

            try {
                // Commit every session factory that's holding a transactional session
                foreach (SessionFactoryElement sessionFactorySettings in openSessionInViewSection.SessionFactories) {
                    if (sessionFactorySettings.IsTransactional) {
                        NHibernateSessionManager.Instance.CommitTransactionOn(sessionFactorySettings.FactoryConfigPath);
                    }
                }
            }
            finally {
                // No matter what happens, make sure all the sessions get closed
                foreach (SessionFactoryElement sessionFactorySettings in openSessionInViewSection.SessionFactories) {
                    NHibernateSessionManager.Instance.CloseSessionOn(sessionFactorySettings.FactoryConfigPath);
                }
            }
        }

        private OpenSessionInViewSection GetOpenSessionInViewSection() {
            OpenSessionInViewSection openSessionInViewSection = ConfigurationManager
                .GetSection("nhibernateSettings") as OpenSessionInViewSection;

            Guard.NotNull(openSessionInViewSection);
            return openSessionInViewSection;
        }

        public void Dispose() { }
    }
}
