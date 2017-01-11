using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VwSync
{
    class Service : System.ServiceProcess.ServiceBase
    {
        public Service()
        {
            this.ServiceName = "CAVISync";
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            Exec.Sync();
        }

    }
}
