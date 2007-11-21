using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;

namespace AnjLab.FX.System
{
    /// <summary>
    /// Allows to start services as simple classes (not windows service). Usefull for quick development
    /// </summary>
    public class OpenService : ServiceBase
    {
        private delegate void StartHandler(string[] args);

        public static void Run(OpenService[] servicesToRun, bool asWinService)
        {
            if (asWinService)
                Run(servicesToRun);
            else
                Start(servicesToRun);
        }

        public static void Start(OpenService[] services)
        {
            List<Pair<StartHandler, IAsyncResult>> results = new List<Pair<StartHandler, IAsyncResult>>();
            string [] args = new string[0];

            foreach (OpenService service in services)
            {
                Pair<StartHandler, IAsyncResult> p = new Pair<StartHandler, IAsyncResult>();
                p.A = service.OnStart;
                p.B = p.A.BeginInvoke(args, null, null);
                results.Add(p);
            }

            foreach (Pair<StartHandler,IAsyncResult> result in results)
                result.A.EndInvoke(result.B);
        }

        public static void Stop(OpenService[] services)
        {
            List<Pair<VoidAction, IAsyncResult>> results = new List<Pair<VoidAction, IAsyncResult>>();

            foreach (OpenService service in services)
            {
                Pair<VoidAction, IAsyncResult> p = new Pair<VoidAction, IAsyncResult>();
                p.A = service.OnStop;
                p.B = p.A.BeginInvoke(null, null);
                results.Add(p);
            }

            foreach (Pair<VoidAction, IAsyncResult> result in results)
                result.A.EndInvoke(result.B);
        }
    }
}
