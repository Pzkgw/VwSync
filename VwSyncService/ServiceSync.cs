using System.Diagnostics;
using System.ServiceProcess;

namespace VwSyncService
{
    public partial class ServiceSync : ServiceBase
    {
        //EventLog eventLogSync;
        public ServiceSync()
        {
            InitializeComponent();
            ServiceName = "CAVISync";
           
            CanHandleSessionChangeEvent = true;
            CanHandlePowerEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;          
            
            //this.AutoLog = true;
            /*
            eventLogSync = new EventLog("CAVISyncLog");
            if (!EventLog.SourceExists("CAVISync"))
            {
                EventLog.CreateEventSource(
                   "CAVISync", "CAVISyncLog");
            }
            eventLogSync.Source = "CAVISync";
            eventLogSync.Log = "CAVISyncLog";*/
        }

        // public void Start() {    OnStart(new string[0]);    }           

        protected override void OnStart(string[] args)
        {
            //base.OnStart(args);
            //System.Diagnostics.Debugger.Launch();
            //eventLogSync.WriteEntry("OnStart");
        }

        protected override void OnStop()
        {
            //eventLogSync.WriteEntry("OnStop");
            //base.OnStop();
        }

        protected override void OnContinue()
        {
            //eventLogSync.WriteEntry("OnContinue");
            //base.OnContinue();
        }


    }
}
