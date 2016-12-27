
using System.ServiceProcess;

namespace VwSyncSever
{
    class Service: ServiceBase
    {

        public Service()
        {
            ServiceName = "SyncServerService";
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
        }


        protected override void OnStart(string[] args)
        {
        }

        }
}
