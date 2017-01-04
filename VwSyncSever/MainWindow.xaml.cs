﻿using System;
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

            textBox2.Text = Settings.dirLocal;
            atextBox2.Text = Settings.dirRemote;

            Settings.dirLocalSync = Settings.dirLocal + '\\' + Settings.GetDirLocalSync();
            UpdateSyncPathGui(true, textBox2.Text, atextBox2.Text);

            textBox.Text = Utils.GetLocalIpAddress();
        }


        private void UpdateSyncPathGui(bool updateGui, string dirLocal, string dirRemote)
        {
            Settings.dirLocal = dirLocal;
            Settings.dirRemote = dirRemote;

            if (updateGui)
            {
                const string f0 = "No files found", f1 = "Files:";
                lblLstServer.Content = ListFiles(Settings.dirLocalSync, lstServerFiles) ? f1 : f0;
                lblLstClient.Content = ListFiles(Settings.dirRemote, lstClientFiles) ? f1 : f0;
            }
        }

        private bool ListFiles(string dir, ListView lst)
        {

            lst.Items.Clear();

            List<string> ff = null;

            if (Directory.Exists(dir)) ff =
                    Utils.GetFilesAndDirectories(dir,
                "___", ".tmp", ".lnk", ".pst");

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

        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            UpdateSyncPathGui(false, textBox2.Text, atextBox2.Text);
            Settings.dirLocalSync = Settings.dirLocal + '\\' + Settings.GetDirLocalSync();

            Settings.optFilter = new FileSyncScopeFilter();

            Settings.optFilter.AttributeExcludeMask = Settings.excludeFileAttributes;
            foreach (string s in Settings.excludeFileExtensions)
                Settings.optFilter.FileNameExcludes.Add(s);

            Settings.directoryStructureIsOk =
                Settings.PrepareDirectories(Settings.dirLocal, Settings.dirRemote);

            SyncOperationStatistics stats = null;
            if (o.InitSync(Settings.optWay, Settings.optFilter, Settings.optFileSync))
            {
                stats = o.Sync();

                //infoLbl.Content = 
                MessageBox.Show(
                    string.Format(
                        "Start Time: {0}{2} End Time: {1}{2} DownloadChangesTotal: {3}{2} DownloadChangesApplied: {4}{2} DownloadChangesFailed: {5}{2} UploadChangesTotal: {6}",
                    stats.SyncStartTime, stats.SyncEndTime, " ",//Environment.NewLine,
                    stats.DownloadChangesTotal, stats.DownloadChangesApplied, stats.DownloadChangesFailed,
                    stats.UploadChangesTotal));

                UpdateSyncPathGui(true, textBox2.Text, atextBox2.Text);
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
