using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;

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

            o = new Orchestrator(new Settings(@"c:\___\", @"c:\__###\SDL1\"));


            UpdateSyncPathGui(o.set);

            o.set.IPServer = Utils.GetLocalIpAddress();
            if (o.set.IPServer != null) lblIpServer.Content = o.set.IPServer.ToString();
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
            //ser.VwRun(); //@"""c:\apache\bin\httpd.exe"" -k runservice"
            if (!Services.IsInstalled(Settings.serName))
            {
                bool started = Services.InstallAndStart(
                                    Settings.serName, "", "VwSync.exe");

                Services.SetDescriereServiciu(Settings.serName, Settings.serDesc);

                infoLbl.Content = "Service " + (started ? "" : "was not") + "started";

                if(started)
                {

                    Services.Start(Settings.serName, 1000);
                }

            }
            else
            {
                infoLbl.Content = Settings.serName + " is already installed";
            }

        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            SyncOperationStatistics stats =
            o.Sync(textBox2.Text, atextBox2.Text);

            if (stats == null)
            {
                if (!o.set.directoryStructureIsOk)
                    infoLbl.Content = "Failed sync. " +
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

            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            o.CleanUp();
        }

        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
