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

        public Orchestrator()
        {

        }

        public bool InitSync(SyncDirectionOrder way,
            FileSyncScopeFilter scopeFilter,
            FileSyncOptions fileSyncOptions)
        {

            bool retVal = false; 


            if (!Settings.directoryStructureIsOk) return retVal;

            try
            {
                Settings.SetupDirectoryStruct();

                // Create file system providers
                localPro = new FileSyncProvider(Settings.dirLocalSync, scopeFilter, fileSyncOptions,
                    Settings.metadataDirectoryPath, Settings.metaLocalFile, Settings.tempDirectoryPath, Settings.pathToSaveConflictLoserFiles);
                remotePro = new FileSyncProvider(Settings.dirRemote, scopeFilter, fileSyncOptions,
                    Settings.metadataDirectoryPath, Settings.metaRemoteFile, Settings.tempDirectoryPath, Settings.pathToSaveConflictLoserFiles);

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
            Show("Total work: " + e.CompletedWork.ToString());
        }

        public SyncOperationStatistics Sync()
        {
            if (!Settings.directoryStructureIsOk) return null;

            SyncOperationStatistics retVal = null;

            if (orchestrator != null)
            {
                //try                {
                retVal = orchestrator.Synchronize();
                //}                catch                {                }
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
