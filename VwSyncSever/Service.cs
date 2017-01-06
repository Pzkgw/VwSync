
using System.ServiceProcess;

namespace VwSyncSever
{
    class Service : ServiceBase
    {
        RegistryService reg;
        public Service()
        {
            ServiceName = "CAVISync";
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = false;
        }

        public void VwRun()
        {
            ServiceBase.Run(this);
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            //System.Windows.MessageBox.Show(RegistryService.GetSyncPath());
        }

    }
}
