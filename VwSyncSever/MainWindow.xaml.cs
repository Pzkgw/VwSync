using System;
using System.Collections.Generic;
using System.IO;
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

        // 1. SETARI
        // Folders to be synced
        string
            /* local */   strLocalFolder = @"c:\\___\",
             /* remote */ strRemoteFolder =
        @"\\CJ-PC\Users\Default\AppData";
        //@"\\10.10.10.47\video\gi test\demo\";
        //@"c:\_ToDo\TestHik\TestHik\bin\x86\Debug\DbgMessages\";

        string[] syncExcludeExtensions = new string[] { "*.tmp", "*.lnk", "*.pst" };
        const string displayExcludeExtension = "metadata";



        public MainWindow()
        {
            InitializeComponent();


            o = new Orchestrator();
            o.SetInfoShowPodium(infoLbl);

            // 3. GUI
            textBox2.Text = strLocalFolder;
            atextBox2.Text = strRemoteFolder;

            UpdateFileList();

            textBox.Text = Utils.GetLocalIpAddress();


        }


        private void UpdateFileList()
        {
            const string f0 = "No files found", f1 = "Files:";
            lblLstServer.Content = ListFiles(strLocalFolder, lstServerFiles) ? f1 : f0;
            lblLstClient.Content = ListFiles(strRemoteFolder, lstClientFiles) ? f1 : f0;
        }

        private bool ListFiles(string dir, ListView lst)
        {
            List<string> ff;

            ff = Utils.GetFilesAndDirectories(dir, displayExcludeExtension);
            if (ff != null && ff.Count > 0)
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
            foreach (string s in syncExcludeExtensions) fsFilter.FileNameExcludes.Add(s);

            // 2.3 FILE OPTIONS
            FileSyncOptions fsOptions = new FileSyncOptions();


            // 2.4 DO Eeeet______________

            o.SetDirectories(strLocalFolder, strRemoteFolder);

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
