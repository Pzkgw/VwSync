using System.Windows;
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
            SyncDirectionOrder way = SyncDirectionOrder.Download;
            FileSyncScopeFilter scopeFilter = new FileSyncScopeFilter();
            FileSyncOptions fileSyncOptions = new FileSyncOptions();

            // 2. GUI
            textBox2.Text = folderA;
            atextBox2.Text = folderB;

            // 3. ORCHESTRATE-IT
            Orchestrator o = new Orchestrator();
            o.SetInfoShowPodium(infoLbl);
            o.SetDirectories(folderA, folderB);
            o.InitSync(way, scopeFilter, fileSyncOptions);
            o.Sync();
        }

        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
