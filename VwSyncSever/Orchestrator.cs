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
        FileSyncProvider localPro = null, remotePro = null;

        string localDirectory, remoteDirectory;
        //int percentSync;
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

            bool retVal = false;

            string rFileName = null;


            switch (remoteDirectory[1])
            {
                case ':':// Windows local prezent la inspectie
                    rFileName = remoteDirectory.Replace('\\', '$').Remove(1, 1);
                    break;
                case '\\':
                    if (remoteDirectory[0] == '\\') // network Windows device
                        rFileName = remoteDirectory.Replace('\\', '$');
                    break;
                default:
                    break;
            }


            if (!startSyncAllowed || rFileName == null) return retVal;

            try
            {
                // setari aditionale pt remote
                string metadataDirectoryPath = localDirectory + "\\_Remet",//"filesync.metadata",//
                 metadataFileName = rFileName,
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
            catch //(Exception ex)
            {
                // Show("Sync fail: " + ex.ToString());

                CleanUp();

                retVal = false;
            }
            finally
            {

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
                try
                {
                    retVal = orchestrator.Synchronize();
                }
                catch
                {

                }
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

        public void CleanUp()
        {
            if (remotePro != null) remotePro.Dispose();
            if (localPro != null) localPro.Dispose();
        }

        ~Orchestrator()
        {
            CleanUp();
            GC.SuppressFinalize(this);
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
