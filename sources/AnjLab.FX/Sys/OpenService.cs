using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;

namespace AnjLab.FX.Sys
{
    /// <summary>
    /// Allows to start services as simple classes (not windows service). Usefull for quick development
    /// </summary>
    public class OpenService : ServiceBase
    {
        public class ServiceInfo
        {
            private ServiceControllerStatus _winState;
            private string _name;
            private string _displayName;
            private bool _installed;

            public ServiceControllerStatus WinState
            {
                get { return _winState; }
                set { _winState = value; }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string DisplayName
            {
                get { return _displayName; }
                set { _displayName = value; }
            }

            public bool Installed
            {
                get { return _installed; }
                set { _installed = value; }
            }
        }

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

        public ServiceController GetServiceController()
        {
            return new ServiceController(ServiceName);
        }

        public ServiceInfo GetInfo()
        {
            ServiceInfo info = new ServiceInfo();
            ServiceController sc = GetServiceController();
            info.Name = ServiceName;
            try
            {
                info.WinState = sc.Status;
                info.DisplayName = sc.DisplayName;
                info.Installed = true;
            } 
            catch (InvalidOperationException)
            {
                info.Installed = false;    
            }
            return info;
        }

        public void Install()
        {
            ServiceProcessInstaller spi = new ServiceProcessInstaller();
            spi.Account = ServiceAccount.LocalSystem;
            spi.Username = null;
            spi.Password = null;
            spi.Context = new InstallContext("install.txt", new string[0]);
            spi.Context.Parameters["assemblypath"] = GetType().Assembly.Location;
            AddInstaller(spi);
            spi.Install(new Hashtable());
        }

        public void AddInstaller(ServiceProcessInstaller spi)
        {
            ServiceInstaller installer = new ServiceInstaller();
            installer.StartType = ServiceStartMode.Automatic;
            installer.ServiceName = ServiceName;
            if (DependedOn != null)
                installer.ServicesDependedOn = DependedOn;
            spi.Installers.Add(installer);
        }

        public void StartWinService()
        {
            ServiceInfo info = GetInfo();
            Guard.IsTrue(info.Installed, "Service should be installed");

            ServiceController sc = GetServiceController();
            if (info.WinState == ServiceControllerStatus.Stopped)
                sc.Start();
            else if (info.WinState == ServiceControllerStatus.Paused)
                sc.Continue();
        }

        public void Uninstall()
        {
            ServiceProcessInstaller spi = new ServiceProcessInstaller();
            ServiceInstaller installer = new ServiceInstaller();
            spi.Installers.Add(installer);
            installer.Context = new InstallContext("uninstall.txt", new string[0]);
            installer.Context.Parameters["assemblypath"] = GetType().Assembly.Location;
            installer.ServiceName = ServiceName;
            installer.Uninstall(null);
        }

        public static void Uninstall(IEnumerable<OpenService> services)
        {
            foreach(var service in services)
            {
                service.Uninstall();
            }
        }

        public static void Install(Type exeType, IEnumerable<OpenService> services, string username, string password)
        {
            var spi = new ServiceProcessInstaller
                          {
                              Account = string.IsNullOrEmpty(username) ? ServiceAccount.LocalSystem : ServiceAccount.User,
                              Username = username,
                              Password = password,
                              Context = new InstallContext("install.txt", new string[0])
                          };

            spi.Context.Parameters["assemblypath"] = exeType.Assembly.Location;
            
            foreach (OpenService service in services)
                service.AddInstaller(spi);

            spi.Install(new Hashtable());
        }

        public static void Install(Type exeType, IEnumerable<OpenService> services)
        {
            Install(exeType, services, null, null);
        }

        /// <summary>
        /// List <code>services</code> to the <code>output</code> using ASCII graphics.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="output"></param>
        public static void ListServices(ICollection<OpenService> services, TextWriter output)
        {
            Guard.ArgumentNotNull("services", services);

            output.WriteLine("------------------------------+-----------+-----------+-------------------------");
            output.WriteLine(" Service Name                 | Installed | State     | Display Name");
            output.WriteLine("------------------------------+-----------+-----------+-------------------------");
            foreach (OpenService service in services)
            {
                OpenService.ServiceInfo info = service.GetInfo();
                output.WriteLine(" {0, -29}| {1, -10}| {2, -10}| {3}",
                    info.Name, info.Installed, info.WinState, info.DisplayName);
            }
            output.WriteLine("------------------------------+-----------+-----------+-------------------------");
        }

        public static void RunServicesInConsole(OpenService[] services, TextReader input, TextWriter output)
        {
            output.WriteLine("Running in console mode");
            output.Write("Starting {0} services...", services.Length);

            Run(services, false);

            output.WriteLine(" done");

            ListServices(services, output);

            output.WriteLine("Press enter to stop services");
            input.ReadLine();

            output.WriteLine("Stopping services...");
            
            Stop(services);
        }

        public virtual string [] DependedOn
        {
            get { return null; }
        }

    }
}
