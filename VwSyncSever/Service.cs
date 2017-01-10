
using System;
using System.ServiceProcess;
using VwSyncService;

namespace VwSyncSever
{
    class Service
    {
        //RegistryService reg;

        ServiceBase[] ServicesToRun;
        public Service()
        {
            ServicesToRun = new ServiceBase[]
{
            new ServiceSync()
};
        }

        public void VwRun()
        {
            
            if (false)//Environment.UserInteractive)
            {
                
            }
            else
            {
                //#if (!DEBUG)      #else       #endif
                ServiceBase.Run(ServicesToRun);
            }
            //ServiceController sc = new ServiceController();
        }


    }
}
