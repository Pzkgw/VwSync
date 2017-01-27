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

        Orchestrator o;

        public MainWindow()
        {
            InitializeComponent();

            o = new Orchestrator(new Settings(@"c:\_sync\", @"\\10.10.10.47\video\gi test"));

            UpdateSyncPathGui(o.set);

            o.set.IPServer = Utils.GetLocalIpAddress();
            if (o.set.IPServer != null) lblIpServer.Content = o.set.IPServer.ToString();

            //MessageBox.Show(Application.ResourceAssembly.Location);
            if (File.Exists(Settings.serExe))
            {
                string s = Application.ResourceAssembly.Location;
                Settings.serExe = s.Substring(0, s.LastIndexOf('\\') + 1) + Settings.serExe;
                //infoLbl.Content = Settings.serExecutabil;

                SetServiceGui(Services.IsInstalled(Settings.serName)); // 
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
            textBox2.Text = set.dirLocal.Substring(0, set.dirLocal.Length - 1);
            atextBox2.Text = set.dirRemote.Substring(0, set.dirRemote.Length - 1);

            const string f0 = "No files found", f1 = "Files:";
            lblLstServer.Content = ListFiles(set.dirLocalSync, lstServerFiles) ? f1 : f0;
            lblLstClient.Content = ListFiles(set.dirRemote, lstClientFiles) ? f1 : f0;

        }

        private bool ListFiles(string dir, ListView lst)
        {

            lst.Items.Clear();

            List<string> ff = null;

            if (Directory.Exists(dir)) ff =
                    Utils.GetFilesAndDirectories(dir,
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
            string path = textBox2.Text;
            bool btnCall = !(sender == null && e == null);

            if (!Directory.Exists(path) || !Utils.DirectoryExists(path))
            {
                if (btnCall) infoLbl.Content = "Cannot start service with the current local path";
                return;
            }

            if (btnCall)
            {
                if (!Verify.LocalDirCheck(path))
                {
                    infoLbl.Content = "Synchronize at least one path. Void service not started.";
                    return;
                }
            }

            if (!Exec.SerIsOn())
            {

                RegistryLocal.Update(null, Settings.port, Guid.Empty, path);



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
                    System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()+
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
                infoLbl.Content = Settings.serName + " is already installed";
            }

        }





        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            //VwService.SerSettings.dirLocal = textBox2.Text; // TODO: ver ca e path valabil
            //VwService.FileStructClass.RunSync();

            //return;

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

            if (Exec.SerIsOn()) System.Threading.Thread.Sleep(2000);

            SyncOperationStatistics stats =
            o.Sync(textBox2.Text, atextBox2.Text);

            if (stats == null)
            {
                infoLbl.Content = "Failed sync. ";

                if (o.set.directoryStructureIsOk && !o.set.remotePathIsOk)
                {
                    infoLbl.Content += string.Format("Remote path should not contain the '{0}' character", Settings.chSlash);
                }

                if (!o.set.directoryStructureIsOk)
                    infoLbl.Content +=
                        (Utils.DirectoryExists(o.set.dirRemote) ? "Directory exists, but without rights." : "Directory does not exist.");
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

                //o.reg.UpdateDeriv(1525, 5000, 
                //    Settings.dirLocalSync.Substring(
                //        Settings.dirLocalSync.LastIndexOf('\\')));

                if (serviceWasRunning && !Exec.SerIsOn()) btnService_Click(null, null);
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
        }
    }
}
