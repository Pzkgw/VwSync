﻿using System.Collections.Generic;
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

        private void Orchestrate()
        {

            // 1. SETARI

            // Folders to be synced
            string
                /* local */   strLocalFolder = @"c:\\___\",
                 /* remote */ strRemoteFolder = @"\\CJ-PC\Users\Default\AppData";
            //@"\\10.10.10.47\video\gi test\demo\";

            string[] syncExcludeExtensions = new string[] { "*.tmp", "*.lnk", "*.pst" };
            string displayExcludeExtension = "metadata";

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

            foreach (string s in Utils.GetFilesAndDirectories(strLocalFolder, displayExcludeExtension)) lstServerFiles.Items.Add(s);

            foreach (string s in Utils.GetFilesAndDirectories(strRemoteFolder, displayExcludeExtension)) lstClientFiles.Items.Add(s);

            textBox.Text = Utils.GetLocalIpAddress();

            // 4. ORCHESTRATE-IT
            Orchestrator o = new Orchestrator();

            o.SetInfoShowPodium(infoLbl);
            o.SetDirectories(strLocalFolder, strRemoteFolder);
            //o.InitSync(fsWay, fsFilter, fsOptions);
            //o.Sync();
        }

        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
