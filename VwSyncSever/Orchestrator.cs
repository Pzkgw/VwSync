using System.IO;
using System.Windows.Controls;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;

namespace VwSyncSever
{
    class Orchestrator
    {

        SyncOrchestrator orchestrator;
        
        public Orchestrator()
        {
            
        }


        

        public void Start(string localProvider, string remoteProvider)
        {

            if (Directory.Exists(localProvider) && Directory.Exists(remoteProvider))
            {
                FileSyncProvider firstProvider = new FileSyncProvider(localProvider);
                FileSyncProvider secondProvider = new FileSyncProvider(remoteProvider);

                orchestrator = new SyncOrchestrator();
                orchestrator.LocalProvider = firstProvider;
                orchestrator.RemoteProvider = secondProvider;
                orchestrator.Direction = SyncDirectionOrder.DownloadAndUpload;
            }
            else
            {
                Show("Dir !exists");
            }


            

        }


        public void DoEeet()
        {
            if (orchestrator != null) {
                orchestrator.Synchronize();
            }
            else
            {
                Show("orchestrator !exists");
            }
            
        }



        #region GUI_STUFF

        Label s;
        public void SetInfoShowPodium(Label inf)
        {
            s = inf;
        }


        private void Show(string mesaj)
        {
            s.Content = mesaj;
        }


        #endregion

    }
}
