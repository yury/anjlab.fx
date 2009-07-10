using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.ServiceModel.Description;
using AnjLab.FX.IO;

namespace AnjLab.FX.Net.Behaviors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HandleErrorsBehavior : Attribute, IErrorHandler, IEndpointBehavior
    {
        [ThreadStatic]
        public static ILog Log;

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var exc = new FaultException(error.InnerException != null ? error.InnerException.Message : error.Message);
            fault = Message.CreateMessage(version, exc.CreateMessageFault(), string.Empty);
        }

        public bool HandleError(Exception error)
        {
            if (Log != null)
            {
                Log.Error("Error handled: {0}", error);
            }
            return true;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (channelDispatcher != null)
            {
                channelDispatcher.ErrorHandlers.Add(this);
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }
}