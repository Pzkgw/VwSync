using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Synchronization;
using VwSyncSever;

namespace VwSer
{
    public partial class VwService : ServiceBase
    {

        private Timer tim;

        public static double ultimulTimpTotalDeExecutie; // in milisecunde


        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public VwService()
        {
            InitializeComponent();
            ServiceName = Settings.serName;

            //this.EventLog.Source = "Un Serviciu";
            //this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;

            //if (!EventLog.SourceExists("Un text pe aici"))
            //    EventLog.CreateEventSource("Un text pe aici", "Application");
        }


        /// <summary>
        /// OnStart: Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            if (SerSettings.run) return;

            if (SerSettings.debug) Lib.WrLog("_Synchronization service start. Local dir: " + SerSettings.dirLocal);

            SerSettings.dirLocal = RegistryLocal.GetLocalPath();

            base.OnStart(args);

            //RunSync();

            tim = new Timer();
            tim.Interval = 1000;
            tim.Elapsed += Tim_Elapsed;
            tim.Start();
        }

        /// <summary>
        /// OnStop: Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();

            tim.Enabled = false;
            if (SerSettings.debug) Lib.WrLog("_Synchronization stop");
        }

        private void Tim_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SerSettings.run) return;

            //if (SerSettings.debug) Lib.WrLog(" timer_elapsed ");

            RunSync();

            ultimulTimpTotalDeExecutie *= 3;
            if (ultimulTimpTotalDeExecutie < 1000) ultimulTimpTotalDeExecutie = 1000;
            if (ultimulTimpTotalDeExecutie > uint.MaxValue) ultimulTimpTotalDeExecutie = uint.MaxValue;
            tim.Interval = ultimulTimpTotalDeExecutie;
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// OnContinue: Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcase Status (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event from a Terminal Server session.
        ///   Useful if you need to determine when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription"></param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }




        public bool RunSync()
        {
            SerSettings.run = true;

            bool retVal = false;

            retVal = (SerSettings.dirLocal != null &&
                Directory.Exists(SerSettings.dirLocal));

            if (retVal)
            {
                string rs;
                int startIdx, count = 0, execTime;

                Orchestrator o;
                SyncOperationStatistics stats;

                ultimulTimpTotalDeExecutie = 0;

                foreach (string s in Directory.GetDirectories(SerSettings.dirLocal))
                {
                    startIdx = s.LastIndexOf('\\') + 1;
                    if (startIdx > 0)
                    {
                        rs = s.Substring(startIdx, s.Length - startIdx);

                        if (rs.Contains(Settings.chSlash))
                        {
                            rs = rs.Replace(Settings.chSlash, '\\');
                            if (rs[0] != '\\') rs = rs.Insert(1, ":"); // director local
                            //Lib.WrLog(string.Format(":: {1} {2} {0} ", rs, Directory.Exists(rs), Utils.DirectoryExists(rs)));
                            bool res =
                                (Directory.Exists(rs) && Utils.DirectoryExists(rs));
                                //VwSync.Imperson.DoWorkUnderImpersonation(rs);


                            if (res)//res)//(rs[0] == '\\') || 
                            {
                                ++count;
                                //Lib.WrLog("CHK2" + rs);
                                o = new Orchestrator(new Settings(SerSettings.dirLocal, rs));
                                string
                                    s1 = (o.GetIdLocal() == null) ? "null" : o.GetIdLocal().ToString(),
                                    s2 = (o.GetIdRemote() == null) ? "null" : o.GetIdRemote().ToString();
                                Lib.WrLog(string.Format("{0} {1} :: ===>{2} {3} {4}",
                                        res, rs, VwSync.Imperson.mesaj, s1, s2));

                                stats = o.Sync(SerSettings.dirLocal, rs);

                                execTime = 0;

                                if (stats != null)
                                    execTime = stats.SyncEndTime.Subtract(stats.SyncStartTime).Milliseconds;

                                ultimulTimpTotalDeExecutie += execTime;

                                if (SerSettings.debug)
                                {
                                    Lib.WrLog(string.Format("Done at {0} in {1}ms", rs, execTime));
                                    /*
                                    Lib.WrLog(string.Format(
                                        " Task done in {0}ms.  Download changes total:{1}  Download changes applied:{2}  Download changes failed:{3}, UploadChangesTotal: {4}",
                                        stats.SyncEndTime.Subtract(stats.SyncStartTime).Milliseconds,
                                        stats.DownloadChangesTotal, stats.DownloadChangesApplied, stats.DownloadChangesFailed,
                                        stats.UploadChangesTotal)); */
                                }

                                if (!SerSettings.run) return false;
                            }
                        }
                    }
                }

                retVal = count > 0;
            }

            SerSettings.run = false;
            return retVal;
        }







    }
}
