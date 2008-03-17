using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

using AnjLab.FX.IO;
using AnjLab.FX.System;
using AnjLab.FX.Tools.IO;

namespace AnjLab.FX.Tools
{
    public static class Library
    {
        private static readonly TimeSpan MessageSentTimeout = TimeSpan.FromMinutes(10);
        private const string RelativePathToConfig = @"Properties\log4net.config";

        static Library()
        {
            LoadLog4NetConfig();
        }

        private static void LoadLog4NetConfig()
        {
            string pathToConfig = Path.Combine(Path.GetDirectoryName(typeof(Library).Assembly.Location), RelativePathToConfig);
            if (!File.Exists(pathToConfig))
            {
                pathToConfig = Path.Combine(Environment.CurrentDirectory, RelativePathToConfig);
            }

            if(!File.Exists(pathToConfig))
            {
                pathToConfig = Path.Combine(Path.GetDirectoryName(new Uri(typeof(Library).Assembly.CodeBase).AbsolutePath), RelativePathToConfig);
            }

            var fi = new FileInfo(pathToConfig);
            if (fi.Exists)
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            } 
            else
            {
                log4net.Config.BasicConfigurator.Configure();
            }
        }

        public static ILog NewLog(string name)
        {
            Guard.ArgumentNotNullNorEmpty("name", name);
            return new Log4NetLog(name);
        }

        public static ILog NewNamespaceLog(Type anyTypeInNamespace)
        {
            Guard.ArgumentNotNull("anyTypeInNamespace", anyTypeInNamespace);
            return new Log4NetLog(anyTypeInNamespace.Namespace);
        }

        public static ServiceChannel<TChannel> CreateChannel<TChannel>(Uri endpoint)
        {
            Guard.ArgumentNotNull("endpoint", endpoint);

            return new ServiceChannel<TChannel>(ChannelFactory<TChannel>.CreateChannel(CreateBindingFromScheme(endpoint), new EndpointAddress(endpoint)));
        }

        public static ServiceChannel<TChannel> InitChannel<TChannel>(out ServiceChannel<TChannel> channel, Uri endpoint)
        {
            return channel = CreateChannel<TChannel>(endpoint);
        }

        public static Binding CreateBindingFromScheme(Uri uri)
        {
            Guard.ArgumentNotNull("uri", uri);
            
            switch (uri.Scheme.ToLower())
            {
                case "net.tcp": 
                    var netTcp = new NetTcpBinding(SecurityMode.None);
                    netTcp.MaxReceivedMessageSize = int.MaxValue;
                    netTcp.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    netTcp.ReaderQuotas.MaxArrayLength = int.MaxValue;
                    netTcp.SendTimeout = MessageSentTimeout;
                    return netTcp;

//                service can't call application wcf service thought net.pipe on vista
//                see http://blogs.thinktecture.com/cweyer/archive/2007/12/07/415050.aspx for details
                case "net.pipe":
                    var netPipe = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                    netPipe.MaxReceivedMessageSize = int.MaxValue;
                    netPipe.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    netPipe.ReaderQuotas.MaxArrayLength = int.MaxValue;
                    netPipe.SendTimeout = MessageSentTimeout;

                    return netPipe;
                case "http": 
                    var http =  new BasicHttpBinding();
                    http.MaxReceivedMessageSize = int.MaxValue;
                    http.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    http.ReaderQuotas.MaxArrayLength = int.MaxValue;
                    http.SendTimeout = MessageSentTimeout;
                    return http;
                default: throw new InvalidOperationException();
            }
            
        }
    }
}
