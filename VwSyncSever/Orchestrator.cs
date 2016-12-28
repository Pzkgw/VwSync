using System;
using System.IO;
using System.Windows.Controls;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;

namespace VwSyncSever
{
    partial class Orchestrator
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

        public bool InitSync(SyncDirectionOrder way,
            FileSyncScopeFilter scopeFilter,
            FileSyncOptions fileSyncOptions)
        {
            if (!startSyncAllowed) return false;

            bool retVal = false;

            FileSyncProvider
                localPro = null,
                remotePro = null;

            try
            {
                // setari aditionale pt remote
                string metadataDirectoryPath = localDirectory + "\\_Remet",
                 metadataFileName = remoteDirectory.Replace('\\','$')+".metadata",
                 tempDirectoryPath = localDirectory + "\\_Temp",
                 pathToSaveConflictLoserFiles = localDirectory + "\\_Conflict";

                if (!Directory.Exists(metadataDirectoryPath)) Directory.CreateDirectory(metadataDirectoryPath);
                if (!Directory.Exists(tempDirectoryPath)) Directory.CreateDirectory(tempDirectoryPath);
                if (!Directory.Exists(pathToSaveConflictLoserFiles)) Directory.CreateDirectory(pathToSaveConflictLoserFiles);

                // Create file system providers
                localPro = new FileSyncProvider(localDirectory, scopeFilter, fileSyncOptions);
                remotePro = new FileSyncProvider(remoteDirectory, scopeFilter, fileSyncOptions,
                    metadataDirectoryPath, metadataFileName, tempDirectoryPath, pathToSaveConflictLoserFiles);

                // Ask providers to detect changes
                localPro.DetectChanges();
                remotePro.DetectChanges();

                // Init Sync
                orchestrator = new SyncOrchestrator();
                orchestrator.LocalProvider = localPro;
                orchestrator.RemoteProvider = remotePro;
                orchestrator.Direction = way;
                orchestrator.SessionProgress += Orchestrator_SessionProgress;
                retVal = true;
            }
            catch (Exception ex)
            {
                Show("Sync fail: " + ex.ToString());
                retVal = false;
            }
            finally
            {
                if (localPro != null) localPro.Dispose();
                if (remotePro != null) remotePro.Dispose();                
            }

            return retVal;
        }

        private void Orchestrator_SessionProgress(object sender, SyncStagedProgressEventArgs e)
        {
            Show(e.TotalWork.ToString());
        }

        public SyncOperationStatistics Sync()
        {
            if (!startSyncAllowed) return null;

            SyncOperationStatistics retVal = null;

            if (orchestrator != null)
            {
                retVal = orchestrator.Synchronize();
            }
            else
            {
                Show("orchestrator !exists");
            }

            return retVal;
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
            //percentSync = v;
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
