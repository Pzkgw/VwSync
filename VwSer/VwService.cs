using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using Microsoft.Synchronization;
using VwSyncSever;

namespace VwSer
{
    public partial class VwService : ServiceBase
    {

        bool retVal = false, mapNetwork, isHatched = false, res;
        int idx, count = 0, execTime;
        public static double ultimulTimpTotalDeExecutie; // in milisecunde

        private string aux, rs;
        private Timer tim;
        private NetworkDrive l;
        private Egg egg;
        private EggArray eggs;

        Orchestrator o;
        SyncOperationStatistics stats;

        string
            usr = "GI",
            pas = "1qaz@WSX";


        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public VwService()
        {
            InitializeComponent();
            ServiceName = Settings.serNameLoc;

            //this.EventLog.Source = "Un Serviciu";
            //this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.


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

            l = new NetworkDrive();
            eggs = new EggArray();

            SerSettings.errCount = 0;

            base.OnStart(args);

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
            Lib.WrLog("_Synchronization stop");
        }

        private void Tim_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SerSettings.run) return;

            GC.Collect();

            ultimulTimpTotalDeExecutie = 0;

            aux = RegistryLocal.GetLocalPath();

            if (aux != null)
            {
                if (SerSettings.dirLocal == null || !SerSettings.dirLocal.Equals(aux))
                {
                    SerSettings.logPathInit = false;
                    SerSettings.dirLocal = aux;

                    /*
                    // verifica ce map drive e liber
                    for(int i=0;i< Settings.mapNetDrives.Length;i++)
                    {
                        if(!Utils.IsDriveMapped(Settings.mapNetDrives[i]))
                        {
                            Settings.mapNetIdx = i;
                            i = 100;
                        }
                    }
                    */

                }
            }

            try
            {
                RunSync();
            }
            catch (Exception ex)
            {
                Lib.WrLog(ex);
                ++SerSettings.errCount;
                if (SerSettings.errCount > SerSettings.errCountMax)
                {
                    Stop();
                }
            }

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
            count = 0;

            retVal = (SerSettings.dirLocal != null &&
                Directory.Exists(SerSettings.dirLocal));

            if (retVal)
            {
                eggs.Start();

                foreach (string s in Directory.GetDirectories(SerSettings.dirLocal))
                {
                    mapNetwork = false;
                    egg = eggs.GetEgg(s);
                    isHatched = (egg != null);

                    idx = 0;

                    if (!isHatched)
                        idx = s.LastIndexOf(Settings.backSlash) + 1; // path length

                    if (idx > 0 || isHatched)
                    {
                        if (!isHatched) rs = s.Substring(idx, s.Length - idx);

                        if (rs.Contains(Settings.chSlash) || isHatched)
                        {
                            if (!isHatched)
                            {
                                rs = rs.Replace(Settings.chSlash, Settings.backSlash);

                                if (rs[0] != Settings.backSlash) rs = rs.Insert(1, ":"); // director local

                                //Checks if the last character is \ as this causes error on mapping a drive.
                                if (rs.Substring(rs.Length - 1, 1) == @"\")
                                {
                                    rs = rs.Substring(0, rs.Length - 1);
                                }
                            }
                            else
                            {
                                rs = egg.orc.set.dirRemote;
                            }

                            res = (Directory.Exists(rs) && Utils.DirectoryExists(rs));

                            //Lib.WrLog(string.Format(":--> {0} ", rs));

                            // && (Char.IsNumber(rs[2]) || Char.IsNumber(rs[3]))
                            if (Utils.IsRemotePath(rs)) //  ---> TRULLY REMOTE <--- 
                            {

                                if (Utils.IsDriveMapped(Settings.mapNetDrives[Settings.mapNetIdx] + Settings.backSlash))
                                {
                                    //Utils.ExecuteCommand(string.Format("net use {0} /delete", Settings.mapNetDrive));
                                    l.DisconnectNetworkDrive(Settings.mapNetDrives[Settings.mapNetIdx], true);

                                    // System.Threading.Thread.Sleep(100);
                                }

                                //DriveSettings.MapNetworkDrive(Settings.mapNetDrive, rs, usr, pas);
                                //Utils.ExecuteCommand(string.Format("net use {0} {1} /user:{2} {3} /persistent:no", Settings.mapNetDrive, rs, usr, pas));
                                //Utils.ExecuteCommand("net use W: \"\\\\10.10.10.47\\video\\gi test\" /user:GI 1qaz@WSX");
                                //$$10.10.10.47$video$gi test
                                //\\10.10.10.47\video\gi test

                                //DriveSettings.MapNetworkDrive("W", "\\\\10.10.10.47\\video\\gi test", "GI", "1qaz@WSX");


                                //string md = ;// + "\\" + s;

                                if (l.MapNetworkDrive(rs, Settings.mapNetDrives[Settings.mapNetIdx], usr, pas) == 0)
                                {
                                    mapNetwork = true;
                                }
                                else
                                {

                                }


                                res = true;

                                //rs = "W:";
                            }

                            //VwSync.Imperson.DoWorkUnderImpersonation(rs);

                            if (res)
                            {
                                ++count;
                                if (isHatched)
                                {
                                    o = egg.orc;
                                }
                                else
                                {
                                    o = new Orchestrator(new Settings(SerSettings.dirLocal, rs));
                                }
                                //string
                                //    s1 = (o.GetIdLocal() == null) ? "null" : o.GetIdLocal().ToString(),
                                //    s2 = (o.GetIdRemote() == null) ? "null" : o.GetIdRemote().ToString();
                                //Lib.WrLog(string.Format("{0} {1} :: ===>{2} {3} {4}",
                                //        res, rs, VwSync.Imperson.mesaj, s1, s2));

                                stats = o.Sync(true, SerSettings.dirLocal, rs);

                                execTime = 0;

                                if (stats != null)
                                {
                                    execTime = stats.SyncEndTime.Subtract(stats.SyncStartTime).Milliseconds;

                                    if (!isHatched)
                                    {
                                        egg = new Egg();
                                        egg.lastExecTimeMs = execTime;
                                        egg.dir = s;
                                        egg.orc = o;
                                        egg.isMapped = mapNetwork;

                                        eggs.AddEgg(egg);
                                    }

                                    if (egg != null) egg.wasChecked = true;

                                }

                                ultimulTimpTotalDeExecutie += execTime;

                                Lib.WrLog(string.Format(" done {0} in {1}ms", rs, execTime));


                                //Lib.WrLog("xxmapNetwork0" + mapNetwork.ToString());
                                //if (mapNetwork)
                                //{
                                //    Lib.WrLog("xxmapNetwork1");
                                //    Utils.ExecuteCommand(string.Format("net use {0} /delete", Settings.mapNetDrives[Settings.mapNetIdx]));
                                //    //DriveSettings.DisconnectNetworkDrive(Settings.mapNetDrives[Settings.mapNetIdx], true);
                                //}

                                if (!SerSettings.run) return false;
                            }
                        }
                    }
                }

                retVal = count > 0;

                eggs.Stop();

                Lib.WrLog(eggs.GetCount().ToString());
            }

            SerSettings.run = false;
            return retVal;
        }







    }
}
