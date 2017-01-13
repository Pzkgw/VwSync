//using System;
//using System.ComponentModel;
//using System.Configuration.Install;
//using System.ServiceProcess;

// ----------------- FOLOSIT DOAR PENTRU TESTE

//namespace WindowsService
//{
//    [RunInstaller(true)]
//    public class ServiceInstaller : Installer
//    {
//        /// <summary>
//        /// Public Constructor for WindowsServiceInstaller.
//        /// - Put all of your Initialization code here.
//        /// </summary>
//        public ServiceInstaller()
//        {
//            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
//            System.ServiceProcess.ServiceInstaller serviceInstaller = new System.ServiceProcess.ServiceInstaller();

//            //# Service Account Information
//            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
//            serviceProcessInstaller.Username = null;
//            serviceProcessInstaller.Password = null;

//            //# Service Information
//            serviceInstaller.DisplayName = "My New C# Windows Service";
//            serviceInstaller.Description = "Tralala";
//            serviceInstaller.StartType = ServiceStartMode.Automatic;

//            // This must be identical to the WindowsService.ServiceBase name
//            // set in the constructor of WindowsService.cs
//            serviceInstaller.ServiceName = "CAVISync";

//            Installers.Add(serviceProcessInstaller);
//            Installers.Add(serviceInstaller);
//        }
//    }
//}
