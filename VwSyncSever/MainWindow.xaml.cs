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

            o = new Orchestrator();
            o.SetInfoShowPodium(infoLbl);

            // 3. GUI
            textBox2.Text = Settings.strLocalFolder;
            atextBox2.Text = Settings.strRemoteFolder;

            UpdateFileList();

            textBox.Text = Utils.GetLocalIpAddress();


        }


        private void UpdateFileList()
        {
            const string f0 = "No files found", f1 = "Files:";
            lblLstServer.Content = ListFiles(Settings.strLocalFolder, lstServerFiles) ? f1 : f0;
            lblLstClient.Content = ListFiles(Settings.strRemoteFolder, lstClientFiles) ? f1 : f0;
        }

        private bool ListFiles(string dir, ListView lst)
        {
            List<string> ff = null;
            if (Directory.Exists(dir)) ff = Utils.GetFilesAndDirectories(dir,
                "___", ".tmp", ".lnk", ".pst");
            if (ff != null && ff.Count() > 0)
            {
                lst.Visibility = Visibility.Visible;
                foreach (string s in ff) lst.Items.Add(s);
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

        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {

            // 2. OPTIUNI

            // 2.1 WAY
            SyncDirectionOrder fsWay = SyncDirectionOrder.Download;

            // 2.2 FILTERS
            FileSyncScopeFilter fsFilter = new FileSyncScopeFilter();
            // exclude temporary files
            fsFilter.AttributeExcludeMask = FileAttributes.System | FileAttributes.Hidden;
            foreach (string s in Settings.syncExcludeExtensions) fsFilter.FileNameExcludes.Add(s);

            // 2.3 FILE OPTIONS
            FileSyncOptions fsOptions = new FileSyncOptions();


            // 2.4 DO Eeeet______________
            o.SetDirectories(Settings.strLocalFolder, Settings.strRemoteFolder);

            SyncOperationStatistics stats = null;
            if (o.InitSync(fsWay, fsFilter, fsOptions))
            {
                stats = o.Sync();

                MessageBox.Show(
                    string.Format(
                        "Start Time: {0}{2} End Time: {1}{2} DownloadChangesTotal: {3}{2} DownloadChangesApplied: {4}{2} DownloadChangesFailed: {5}{2} UploadChangesTotal: {6}",
                    stats.SyncStartTime, stats.SyncEndTime, Environment.NewLine,
                    stats.DownloadChangesTotal, stats.DownloadChangesApplied, stats.DownloadChangesFailed,
                    stats.UploadChangesTotal));

                UpdateFileList();
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
