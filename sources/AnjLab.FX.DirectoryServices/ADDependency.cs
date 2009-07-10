using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading;
using AnjLab.FX.Sys;

namespace AnjLab.FX.DirectoryServices
{
    /// <summary>
    /// This code is reworked code from http://msdn.microsoft.com/msdnmag/issues/07/07/SecurityBriefs/?topics=/msdnmag/issues/07/07/SecurityBriefs
    /// </summary>
    public class ADDependency
    {
        public ADDependency(string dnSearchRoot,
                            string ldapFilter,
                            params string[] attrsToWatch)
            : this("localhost", dnSearchRoot, ldapFilter,
                   DirectorySynchronizationOptions.ObjectSecurity,
                   attrsToWatch)
        {
        }

        public ADDependency(string adServer,
                            string dnSearchRoot,
                            string ldapFilter,
            DirectorySynchronizationOptions dirSyncOptions,
                            params string[] attrsToWatch)
        {
            _adServer = adServer;
            _dnSearchRoot = dnSearchRoot;
            _ldapFilter = ldapFilter;
            _attrsToWatch = attrsToWatch;
            _dirSyncOptions = dirSyncOptions;
        }

        public void Start()
        {
            Guard.IsNull(_connection, "You may only call Start one time.");

            _connection = new LdapConnection(
                new LdapDirectoryIdentifier(_adServer),
                null, AuthType.Negotiate);

            _connection.Bind();

            _timer = new Timer(timerCallback, null,
                              TimeSpan.FromSeconds(0),
                              pollingInterval);
        }

        public void Stop()
        {
            _timer.Dispose();
            _connection.Dispose();
        }

        public event EventHandler<EventArgs<SearchResponse>> Changed;

        const int RequestTimeoutInSeconds = 5;
        const int PollingIntervalInSeconds = 10; // low value for demo purposes

        readonly string _adServer;
        readonly string _dnSearchRoot;
        readonly string _ldapFilter;
        LdapConnection _connection;
        readonly string[] _attrsToWatch;
        readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(RequestTimeoutInSeconds);
        readonly TimeSpan pollingInterval = TimeSpan.FromSeconds(PollingIntervalInSeconds);
        byte[] cookie;
        readonly DirectorySynchronizationOptions _dirSyncOptions;
        Timer _timer;
        readonly Guid guid = Guid.NewGuid();

        
        private void timerCallback(object o)
        {
            BeginPollDirectory();
        }

        private void BeginPollDirectory()
        {
            SearchRequest request = new SearchRequest(_dnSearchRoot,
                _ldapFilter, SearchScope.Subtree, _attrsToWatch);
            request.Controls.Add(new DirSyncRequestControl(cookie,
                _dirSyncOptions));
            _connection.BeginSendRequest(request, requestTimeout,
                PartialResultProcessing.NoPartialResultSupport,
                EndPollDirectory, null);
        }

        private void EndPollDirectory(IAsyncResult asyncResult)
        {
            // harvest results
            SearchResponse response =
                (SearchResponse)_connection.EndSendRequest(asyncResult);

            // grab the current search cookie
            foreach (DirectoryControl control in response.Controls)
            {
                DirSyncResponseControl dsrc = control
                    as DirSyncResponseControl;
                if (null != dsrc)
                {
                    cookie = dsrc.Cookie;
                }
            }

            // if anything changed, notify any listeners
            if (response.Entries.Count > 0)
            {
                if (null != Changed)
                {
                    Changed(this, EventArg.New(response));
                }
            }
        }

        public string GetUniqueID()
        {
            return guid.ToString();
        }
    }
}
