using System.ServiceProcess;

namespace AnjLab.FX.System
{
    /// <summary>
    /// Allows to start services as simple classes (not windows service). Usefull for quick development
    /// </summary>
    public class OpenService : ServiceBase
    {
        public static void Run(OpenService[] servicesToRun, bool asWinService)
        {
            if (asWinService)
                Run(servicesToRun);
            else
                Start(servicesToRun);
        }

        public static void Start(OpenService[] services)
        {
            foreach (OpenService service in services)
            {
                service.OnStart(new string[] { });
            }
        }

        public static void Stop(OpenService[] services)
        {
            foreach (OpenService service in services)
            {
                service.OnStop();
            }
        }
    }
}
