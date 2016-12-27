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
        public MainWindow()
        {
            InitializeComponent();

            Orchestrate();
        }


        // 1. SETARI
        // Folders to be synced
        string
            /* local */   strLocalFolder = @"c:\\___\",
             /* remote */ strRemoteFolder = @"\\CJ-PC\Users\Default\AppData";
        //@"\\10.10.10.47\video\gi test\demo\";

        string[] syncExcludeExtensions = new string[] { "*.tmp", "*.lnk", "*.pst" };
        const string displayExcludeExtension = "metadata";


        private void Orchestrate()
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

            // 3. GUI
            textBox2.Text = strLocalFolder;
            atextBox2.Text = strRemoteFolder;

            UpdateFileList();

            textBox.Text = Utils.GetLocalIpAddress();

            // 4. ORCHESTRATE-IT
            Orchestrator o = new Orchestrator();

            o.SetInfoShowPodium(infoLbl);
            o.SetDirectories(strLocalFolder, strRemoteFolder);
            //o.InitSync(fsWay, fsFilter, fsOptions);
            //o.Sync();
        }

        private void UpdateFileList()
        {
            lblLstServer.Visibility = ListFiles(strLocalFolder, lstServerFiles) ? Visibility.Visible : Visibility.Hidden;
            lblLstClient.Visibility = ListFiles(strRemoteFolder, lstClientFiles) ? Visibility.Visible : Visibility.Hidden;
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

        }

        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
