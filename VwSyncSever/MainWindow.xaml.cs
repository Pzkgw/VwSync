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

        private void Orchestrate()
        {

            // 1. SETARI

            // Folders to be synced
            string
                folderA = @"c:\\_\", // remoteProvider
                folderB = @"\\10.10.10.47\video\gi test\demo\" // localProvider
                ;

            // 2. OPTIUNI

            // 2.1 WAY
            SyncDirectionOrder fsWay = SyncDirectionOrder.Download;

            // 2.2 FILTERS
            FileSyncScopeFilter fsFilter = new FileSyncScopeFilter();
            // exclude temporary files
            fsFilter.AttributeExcludeMask = FileAttributes.System | FileAttributes.Hidden;
            fsFilter.FileNameExcludes.Add("*.tmp");
            fsFilter.FileNameExcludes.Add("*.lnk");
            fsFilter.FileNameExcludes.Add("*.pst");

            // 2.3 FILE OPTIONS
            FileSyncOptions fsOptions = new FileSyncOptions();

            // 3. GUI
            textBox2.Text = folderA;
            atextBox2.Text = folderB;

            foreach (string s in Utils.GetFilesAndDirectories(folderA, "metadata")) lstServerFiles.Items.Add(s);

            textBox.Text = Utils.GetLocalIpAddress();

            // 4. ORCHESTRATE-IT
            Orchestrator o = new Orchestrator();
            
            //o.SetInfoShowPodium(infoLbl);
            //o.SetDirectories(folderA, folderB);
            //o.InitSync(fsWay, fsFilter, fsOptions);
            //o.Sync();
        }

        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
