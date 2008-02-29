using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace AnjLab.FX.System
{
    public class ServiceChannel<TServiceContract>: IDisposable
    {
        private readonly TServiceContract _service;

        public ServiceChannel(TServiceContract service)
        {
            Guard.ArgumentNotNull("service", service);
            _service = service;
        }

        public TServiceContract Service
        {
            get { return _service; }
        }

        public void Dispose()
        {
            if (_service == null)
                return;
            var channel = _service as IChannel;
            if (channel != null)
                channel.Close();
        }
    }
}
