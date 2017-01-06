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
            //o.SetInfoShowPodium(infoLbl);

            Settings.dirLocalSync = Settings.dirLocal + Settings.GetDirLocalSync();
            UpdateSyncPathGui(Settings.dirLocal, Settings.dirRemote);

            Settings.IP = Utils.GetLocalIpAddress();
            if(Settings.IP!=null) lblIpServer.Content = Settings.IP.ToString();
            
        }

        void RefreshPaths(string dirLocal, string dirRemote)
        {
            Settings.dirLocal = dirLocal;
            Settings.dirRemote = dirRemote;

            if (!Settings.dirLocal.EndsWith("\\")) Settings.dirLocal = Settings.dirLocal + "\\";
            if (!Settings.dirRemote.EndsWith("\\")) Settings.dirRemote = Settings.dirRemote + "\\";
            if (!Settings.dirLocalSync.EndsWith("\\")) Settings.dirLocalSync = Settings.dirLocalSync + "\\";

        }


        private void UpdateSyncPathGui(string dirLocal, string dirRemote)
        {
                textBox2.Text = Settings.dirLocal.Substring(0, Settings.dirLocal.Length - 1);
                atextBox2.Text = Settings.dirRemote.Substring(0, Settings.dirRemote.Length - 1);

                const string f0 = "No files found", f1 = "Files:";
                lblLstServer.Content = ListFiles(Settings.dirLocalSync, lstServerFiles) ? f1 : f0;
                lblLstClient.Content = ListFiles(Settings.dirRemote, lstClientFiles) ? f1 : f0;
            
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

        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            RefreshPaths(textBox2.Text, atextBox2.Text);

            if (o.reg == null)
            {
                o.reg = new Registry();
                o.reg.UpdateBase(Settings.IP, 4500, Settings.dirLocal);
            }

            Settings.dirLocalSync = Settings.dirLocal + Settings.GetDirLocalSync();

            Settings.optFilter = new FileSyncScopeFilter();

            Settings.optFilter.AttributeExcludeMask = Settings.excludeFileAttributes;
            foreach (string s in Settings.excludeFileExtensions)
                Settings.optFilter.FileNameExcludes.Add(s);

            Settings.directoryStructureIsOk =
                Settings.PrepareDirectories(Settings.dirLocal, Settings.dirRemote);

            if (!Settings.directoryStructureIsOk)
            {

                infoLbl.Content = "Failed sync. " +
                    (Utils.DirectoryExists(Settings.dirRemote) ? "Directory exists, but without rights." : "Directory does not exist.");
            }

            SyncOperationStatistics stats = null;
            if (o.InitSync(Settings.optWay, Settings.optFilter, Settings.optFileSync))
            {
                stats = o.Sync();

                infoLbl.Content =
                    //MessageBox.Show(
                    string.Format(
                        " Task done in {0}ms.  Download changes total:{1}  Download changes applied:{2}  Download changes failed:{3}", //  UploadChangesTotal: {6}
                    stats.SyncEndTime.Subtract(stats.SyncStartTime).Milliseconds, //Environment.NewLine,
                    stats.DownloadChangesTotal, stats.DownloadChangesApplied, stats.DownloadChangesFailed
                    //stats.UploadChangesTotal
                    );

                UpdateSyncPathGui(Settings.dirLocal, Settings.dirRemote);

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
