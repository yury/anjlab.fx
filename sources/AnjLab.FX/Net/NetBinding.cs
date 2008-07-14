using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Net
{
    public static class NetBinding
    {
        private static readonly TimeSpan MessageSentTimeout = TimeSpan.FromMinutes(10);

        public static ServiceChannel<TChannel> CreateChannel<TChannel>(Uri endpoint) where TChannel : class
        {
            Guard.ArgumentNotNull("endpoint", endpoint);

            var c = new ChannelFactory<TChannel>(CreateBindingFromScheme(endpoint),
                                                           new EndpointAddress(endpoint));

            //if (endpoint.Scheme.ToLower() == "https")
            //{
            //    try
            //    {
            //        c.Credentials.ClientCertificate.SetCertificate(
            //            System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser,
            //            System.Security.Cryptography.X509Certificates.StoreName.My,
            //            System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName,
            //            endpoint.Authority);
            //        c.Credentials.ServiceCertificate.SetDefaultCertificate(
            //            System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            //            System.Security.Cryptography.X509Certificates.StoreName.My,
            //            System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName,
            //            endpoint.Authority);
            //    } catch(Exception exc)
            //    {
            //    }
            //}

            return new ServiceChannel<TChannel>(c.CreateChannel());
        }

        public static ServiceChannel<TChannel> InitChannel<TChannel>(out ServiceChannel<TChannel> channel, Uri endpoint)
            where TChannel : class
        {
            return channel = CreateChannel<TChannel>(endpoint);
        }

        public static Binding CreateBindingFromScheme(Uri uri)
        {
            return CreateBindingFromScheme(uri, TransferMode.Buffered);
        }

        public static Binding CreateBindingFromScheme(Uri uri, TransferMode transferMode)
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
                    netTcp.TransferMode = transferMode;
                    return netTcp;

                //                service can't call application wcf service thought net.pipe on vista
                //                see http://blogs.thinktecture.com/cweyer/archive/2007/12/07/415050.aspx for details
                case "net.pipe":
                    var netPipe = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                    netPipe.MaxReceivedMessageSize = int.MaxValue;
                    netPipe.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    netPipe.ReaderQuotas.MaxArrayLength = int.MaxValue;
                    netPipe.SendTimeout = MessageSentTimeout;
                    netPipe.TransferMode = transferMode;
                    return netPipe;
                case "http":
                    var http = new BasicHttpBinding();
                    http.MaxReceivedMessageSize = int.MaxValue;
                    http.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    http.ReaderQuotas.MaxArrayLength = int.MaxValue;
                    http.SendTimeout = MessageSentTimeout;
                    http.TransferMode = transferMode;
                    
                    return http;

                case "https":
                    var https = new BasicHttpBinding();
                    https.MaxReceivedMessageSize = int.MaxValue;
                    https.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    https.ReaderQuotas.MaxArrayLength = int.MaxValue;
                    https.SendTimeout = MessageSentTimeout;
                    https.TransferMode = transferMode;
                    https.Security.Mode = BasicHttpSecurityMode.Transport;
                    https.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;

                    return https;
                default: throw new InvalidOperationException();
            }
        }
    }
}
