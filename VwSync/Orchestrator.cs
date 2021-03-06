﻿using System;
using System.Linq;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;

namespace VwSyncSever
{
    public class Orchestrator
    {
        SyncOrchestrator orchestrator;
        FileSyncProvider
            localPro = null,
            remotePro = null;

        public Settings set;

        //Guid sourceId, destId;
        public Orchestrator(Settings settings)
        {
            set = settings;
        }

        public bool InitSync(
            bool serviceCall,
            SyncDirectionOrder way,
            FileSyncScopeFilter scopeFilter,
            FileSyncOptions fileSyncOptions)
        {

            bool retVal = false;

            set.ErrCount = 0;

            try
            {

                //Generate a unique Id for the source and store it in file or database for refer it further
                //sourceId = NewSyncGuid(); // SyncId()
                //Generate a unique Id for the destination and store it in a file or database for refer it further
                //destId = NewSyncGuid();
                //ReplicaId se genereaza in constructor

                // file sync providers
                localPro = new FileSyncProvider(set.dirLocalSync,
                    scopeFilter, fileSyncOptions,
                      set.metadataDirectoryPath, Settings.metaFileLoc,
                      set.tempDirectoryPath, set.pathToSaveConflictLoserFiles);

                remotePro = new FileSyncProvider(set.dirRemote,
                    scopeFilter, fileSyncOptions,
                    set.metadataDirectoryPath, Settings.metaFileRem,
                    set.tempDirectoryPath, set.pathToSaveConflictLoserFiles);

                // Task curent: Skip delete
                // ChangeType:
                // Create A file or folder will be created
                // Delete A file or folder will be deleted
                // Update A file or folder will be updated
                // Rename A file or folder will be renamed
                remotePro.ApplyingChange += ProviderEvent_ApplyingChange;
                localPro.ApplyingChange += ProviderEvent_ApplyingChange;


                // Ask providers to detect changes
                //FileSyncProvider.DetectChanges( ) is called either implicitly 
                //by the SyncOrchestrator.Synchronize( ) method,
                //or explicitly if the FileSyncProvider options specifies
                //that DetectChanges is to be called explicitly
                DetectChanges();

                //sourceId = localPro.ReplicaId;
                //destId = remotePro.ReplicaId;

                // Init Sync
                orchestrator = new SyncOrchestrator();
                orchestrator.LocalProvider = localPro;
                orchestrator.RemoteProvider = remotePro;
                orchestrator.Direction = way;
                //orchestrator.SessionProgress += Orchestrator_SessionProgress;
                retVal = true;
            }
            catch //(Exception ex)
            {
                //("Sync fail: " + ex.ToString());

                CleanUp();

                retVal = false;

            }
            //finally


            return retVal;
        }

        public void DetectChanges()
        {
            localPro.DetectChanges();
            remotePro.DetectChanges();
        }

        private void ProviderEvent_ApplyingChange(object sender, ApplyingChangeEventArgs e)
        {
            e.SkipChange = (e.ChangeType == ChangeType.Delete);
        }

        /*
        private void Orchestrator_SessionProgress(object sender, SyncStagedProgressEventArgs e)
        {
            Show("Total work: " + e.CompletedWork.ToString());
        }*/

        public SyncOperationStatistics Sync(bool serviceCall, string lStr, string rStr)
        {
            if (set == null) return null;

            // verificari pentru structura de directoare
            set.dirLocalSync = set.dirLocal + Settings.GetDirLocalName(rStr);
            set.RefreshPaths(lStr, rStr);

            set.directoryStructureIsOk =
                set.PrepareDirectories(set.dirLocal, set.dirRemote);

            set.remotePathIsOk = !set.dirRemote.Contains(Settings.chSlash);

            if (!set.directoryStructureIsOk || !set.remotePathIsOk) return null;

            bool metadataDirectoryCreatedNow = false;

            set.SetupDirectoryStruct(ref metadataDirectoryCreatedNow);

            if (!serviceCall)
            {
                string mp;
                mp = string.Format("{0}{1}{2}", set.metadataDirectoryPath, Settings.backSlash, Settings.metaFileRem);
                if (System.IO.File.Exists(mp)) System.IO.File.Delete(mp);
                mp = string.Format("{0}{1}{2}", set.metadataDirectoryPath, Settings.backSlash, Settings.metaFileLoc);
                if (System.IO.File.Exists(mp)) System.IO.File.Delete(mp);
            }

            SyncOperationStatistics retVal = null;

            // sincronizare
            if (InitSync(serviceCall, set.optWay, set.optFilter, set.optFileSync))
            {
                retVal = SyncOperationExecute();
            }

            set.directoryCleanupRequired = (!serviceCall && retVal == null && metadataDirectoryCreatedNow);

            return retVal;
        }

        public SyncOperationStatistics SyncOperationExecute()
        {
            if (!set.directoryStructureIsOk || !set.remotePathIsOk || orchestrator == null) return null;

            return orchestrator.Synchronize();
        }



        #region GUI_STUFF



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
