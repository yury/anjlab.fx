using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Net
{
    public static class NetBinding
    {
        private static readonly TimeSpan MessageSentTimeout = TimeSpan.FromMinutes(10);

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
                    var http = new BasicHttpBinding();
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
