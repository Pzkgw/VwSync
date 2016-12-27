using System;
using System.IO;
using System.Windows.Controls;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;

namespace VwSyncSever
{
    class Orchestrator
    {

        SyncOrchestrator orchestrator;

        string localDirectory, remoteDirectory;
        int percentSync;
        bool startSyncAllowed;

        public Orchestrator()
        {

        }

        public void SetDirectories(string local, string remote)
        {
            localDirectory = local;
            remoteDirectory = remote;
            startSyncAllowed = PrepareDirectories(local, remote);
        }

        public void InitSync(SyncDirectionOrder way,
            FileSyncScopeFilter scopeFilter,
            FileSyncOptions fileSyncOptions)
        {
            if (!startSyncAllowed) return;

            // Create file system providers
            FileSyncProvider providerA = new FileSyncProvider(localDirectory, scopeFilter, fileSyncOptions);
            FileSyncProvider providerB = new FileSyncProvider(remoteDirectory, scopeFilter, fileSyncOptions);

            // Ask providers to detect changes
            providerA.DetectChanges();
            providerB.DetectChanges();

            // Init Sync
            orchestrator = new SyncOrchestrator();
            orchestrator.LocalProvider = providerA;
            orchestrator.RemoteProvider = providerB;
            orchestrator.Direction = way;

        }


        public void Sync()
        {
            if (!startSyncAllowed) return;

            if (orchestrator != null)
            {
                SetPercentSync(0);
                orchestrator.Synchronize();
                SetPercentSync(100);
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

        private void SetPercentSync(int v)
        {
            percentSync = v;
        }

        #endregion



        /// <summary>
        /// Prepare directories for sync
        /// </summary>
        private bool PrepareDirectories(params string[] dir)
        {
            if (dir == null || dir[1] == null)
            {
                Show("No directories to sync ...");
                return false;
            }

            if (!Directory.Exists(dir[0]))
            {
                //Show("No directory for local provider"); return false;
                Directory.CreateDirectory(dir[0]);
            }

            if (!Directory.Exists(dir[1]))
            {
                Show("No directory for remote provider");
                return false;
            }

            //File.WriteAllText(Path.Combine(folderA, "A.txt"), " zbang ... ");

            return true;
        }

        /*
        static void PrepareDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);
            }
            Directory.CreateDirectory(directoryName);
        }*/
    }
}
