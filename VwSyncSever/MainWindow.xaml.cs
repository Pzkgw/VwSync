using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Synchronization;
using VwSer;

namespace VwSyncSever
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Orchestrator o;

        private ConnectionMessage mesaj;

        public string
            pathProviderLocal, pathProviderRemote,
            user, pass;

        public MainWindow()
        {
            InitializeComponent();

            o = new Orchestrator(new Settings(@"c:\_sync", @"\\10.10.10.47\video\gi test"));

            UpdateSyncPathGui(o.set);

            o.set.IPServer = Utils.GetLocalIpAddress();
            if (o.set.IPServer != null) lblIpServer.Content = o.set.IPServer.ToString();

            //MessageBox.Show(Application.ResourceAssembly.Location);
            if (File.Exists(Settings.serExe))
            {
                string s = Application.ResourceAssembly.Location;
                Settings.serExe = s.Substring(0, s.LastIndexOf(Settings.backSlash) + 1) + Settings.serExe;
                //infoLbl.Content = Settings.serExecutabil;

                SetServiceGui(Services.IsInstalled(Settings.serNameLoc)); // 
            }
            else
            {
                btnService.IsEnabled = false;
                infoLbl.Content = "No service start available";
            }
        }

        private void SetServiceGui(bool v)
        {
            serLbl.Content = v ? "on" : "off";
            btnService.Visibility = v ? Visibility.Collapsed : Visibility.Visible;
            btnSerDel.Visibility = v ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateSyncPathGui(Settings set)
        {
            textBox2.Text = set.dirLocal;
            atextBox2.Text = set.dirRemote;

            const string f0 = "", f1 = "Files:";
            //lblLstServer.Content = ListFiles(set.dirLocalSync, lstServerFiles) ? f1 : f0;
            lblLstClient.Content = ListFiles(set.dirRemote, lstClientFiles) ? f1 : f0;

        }

        private bool ListFiles(string dir, ListView lst)
        {

            lst.Items.Clear();

            List<string> ff = null;

            if (Directory.Exists(dir)) ff =
                    Utils.GetFiles(dir,
                "___", Settings.excludeFileExtensions);

            if (ff != null && ff.Count() > 0)
            {
                lst.Visibility = Visibility.Visible;
                foreach (string s in ff) lst.Items.Add(s.Substring(dir.Length));
                return true;
            }
            else
            {
                lst.Visibility = Visibility.Hidden;
                return false;
            }
        }

        private void btnService_Click(object sender, RoutedEventArgs e)
        {
            pathProviderLocal = textBox2.Text;
            bool btnCall = !(sender == null && e == null);

            if (!Directory.Exists(pathProviderLocal) || !Utils.DirectoryExists(pathProviderLocal))
            {
                if (btnCall) infoLbl.Content = "Cannot start service with the current local path";
                return;
            }

            SerSettings.dirLocal = pathProviderLocal;


            if (btnCall)
            {
                if (!Settings.LocalDirCheck(pathProviderLocal))
                {
                    infoLbl.Content = "Synchronize at least one path. Void service not started.";
                    return;
                }
            }

            if (!Exec.SerIsOn())
            {
                RegistryCon.Update(null, -1, Guid.Empty, pathProviderLocal);

                bool started = true;

                //  The switches (username, password, etc) must
                //be placed before the name of the service to be installed, 
                //otherwise the switches will not be used.I made this mistake
                //so I initially thought that these switches were not working :)

                //Use the /? or / help switch to learn more about the other options
                //that can be used in installing the service

                // InstallUtil.exe /? Service.exe

                // /unattended : will not prompt for username and password

                //VwSer.ProjectInstaller l = new VwSer.ProjectInstaller();
                //l.Install();
                Utils.ExecuteCommand(
                    System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() +
                    "\\InstallUtil.exe /i /unattended " +
                    Settings.serExe); //  /user=GI\bogdan.visoiu /password=

                /*
                started = Services.Install(
                                    Settings.serName, Settings.serName, Settings.serExecutabil);

                Services.SetDescriereServiciu(Settings.serName, Settings.serDesc);*/

                if (btnCall) infoLbl.Content = "Service " + (started ? "" : "was not") + "started";


                Exec.SerStart();
                SetServiceGui(true);



            }
            else
            {
                infoLbl.Content = Settings.serNameLoc + " is already installed";
            }

        }





        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            Window_Closing(null, null);
            Close();
        }

        public void btnSync_Click(object sender, RoutedEventArgs e)
        {
            //VwService.SerSettings.dirLocal = textBox2.Text; // TODO: ver ca e path valabil
            //VwService.FileStructClass.RunSync();

            //return;
            bool mapNetwork = false;

            if (mesaj != null)
            {
                mesaj = null;
            }

            if (e == null) // mesaj window call
            {
                // user, pass disponibile
                mapNetwork = ConnectToRemote();
            }
            else // synchronize press call
            {
                pathProviderRemote = atextBox2.Text;
                if (!string.IsNullOrEmpty(textBox2.Text) && 
                    !textBox2.Text.Equals(pathProviderLocal))
                {
                    pathProviderLocal = textBox2.Text;
                    SerSettings.dirLocal = pathProviderLocal;

                    SerSettings.UpdateFileLocations(Settings.backSlash,
                        Settings.serLogFile, Settings.serPasswFile);
                }

                Settings.SearchPasswordFile(
                    SerSettings.passwFilePath,
                    Settings.GetDirLocalName(pathProviderRemote),
                    ref user, ref pass);

                if (!string.IsNullOrEmpty(user)) mapNetwork = ConnectToRemote();

            }

            bool serviceWasRunning = Exec.SerIsOn();
            //Exec.SerStop();
            SerSettings.run = false;
            Exec.SerDelete(serviceWasRunning);
            SetServiceGui(false);

            /*
            // directorul local modificat -> restart serviciul 
            string s1 = RegistryLocal.GetLocalPath(),
                s2 = textBox2.Text;
            if(s1.length==(s2.length+1))
            if(!hh.Equals())
            {
                MessageBox.Show(hh+Environment.NewLine+ textBox2.Text);
                Exec.SerDelete();
            }*/

            if (Exec.SerIsOn()) System.Threading.Thread.Sleep(100);

            SyncOperationStatistics stats = null;


            try
            {
                stats = o.Sync(false, pathProviderLocal, pathProviderRemote);
            }
            catch //(Exception ex)
            {

            }

            // Get stats => show result
            if (stats == null)
            {
                if (e != null)
                {
                    infoLbl.Content = "Synchronization failed";

                    if (o.set.directoryStructureIsOk && !o.set.remotePathIsOk)
                    {
                        infoLbl.Content += string.Format("Remote path should not contain the '{0}' character", Settings.chSlash);
                    }
                    else
                    if (!o.set.directoryStructureIsOk)
                        infoLbl.Content +=
                            (Utils.DirectoryExists(o.set.dirRemote) ? "Directory exists, but without rights." : "Directory does not exist.");
                    else
                    if (Utils.IsValidPath(o.set.dirRemote))
                    {

                        mesaj = new ConnectionMessage();

                        mesaj.OkClick += Mesaj_OkClick;

                        mesaj.lblInfo.Content =
                            string.Format("Connection solutions:{0}{0}" +
                            " - update username and password and click Ok{0}" +
                            " - or before synchronization make sure you can{0}   reach the path from Windows Explorer",
                            Environment.NewLine);

                        if (e != null) mesaj.Show();
                    }
                }
            }
            else
            {

                infoLbl.Content =    //MessageBox.Show(
                    string.Format(
                        " Task done in {0}ms.  Download changes total:{1}  Download changes applied:{2}  Download changes failed:{3}", //  UploadChangesTotal: {6}
                        stats.SyncEndTime.Subtract(stats.SyncStartTime).Milliseconds, //Environment.NewLine,
                        stats.DownloadChangesTotal, stats.DownloadChangesApplied, stats.DownloadChangesFailed
                        //stats.UploadChangesTotal
                        );
                UpdateSyncPathGui(o.set);
            }

            if (mapNetwork)
            {
                PinvokeWindowsNetworking.disconnectRemote(pathProviderRemote);
            }

            // restart service
            if (stats != null && serviceWasRunning && !Exec.SerIsOn()) btnService_Click(null, null);

        }

        private bool ConnectToRemote()
        {
            bool retVal = false;
            string connectStringResult =
    PinvokeWindowsNetworking.connectToRemote(pathProviderRemote, user, pass);

            if (connectStringResult == null)
            {
                retVal = true;
            }
            else
            {
                infoLbl.Content = connectStringResult;

            }
            return retVal;
        }

        private void Mesaj_OkClick(object sender, EventArgs e)
        {
            if (mesaj != null)
            {
                if (!String.IsNullOrEmpty(mesaj.username))
                {
                    user = mesaj.username;
                    pass = mesaj.password;
                    mesaj.Close();
                    btnSync_Click(null, null);
                }
                else
                {
                    infoLbl.Content = string.Empty;
                    mesaj.Close();
                }
            }
        }

        private void btnSerDel_Click(object sender, RoutedEventArgs e)
        {
            infoLbl.Content = string.Empty;
            SerSettings.run = false;
            Exec.SerDelete();

            SetServiceGui(false);
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            o.CleanUp();
            if (mesaj != null)
            {
                mesaj.Close();
                mesaj = null;
            }
        }
    }
}
