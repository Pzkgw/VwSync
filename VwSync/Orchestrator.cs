using System;
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

        public RegistryLocal reg;
        public Settings set;

        Guid sourceId, destId;
        public Orchestrator(Settings settings)
        {
            set = settings;
        }

        public bool InitSync(SyncDirectionOrder way,
            FileSyncScopeFilter scopeFilter,
            FileSyncOptions fileSyncOptions)
        {

            bool retVal = false;

            set.ErrCount = 0;

            if (!set.directoryStructureIsOk || !set.remotePathIsOk) return retVal;

            try
            {
                set.SetupDirectoryStruct();



                //Generate a unique Id for the source and store it in file or database for refer it further
                sourceId = NewSyncGuid(); // SyncId()
                //Generate a unique Id for the destination and store it in a file or database for refer it further
                destId = NewSyncGuid();
                //ReplicaId se genereaza in constructor

              // file sync providers
              localPro = new FileSyncProvider(sourceId, set.dirLocalSync,
                  scopeFilter, fileSyncOptions,
                    set.metadataDirectoryPath, Settings.metaFileLoc,
                    set.tempDirectoryPath, set.pathToSaveConflictLoserFiles);

                remotePro = new FileSyncProvider(destId, (set.dirRemote[0]=='\\')?"W:\\":set.dirRemote,
                    scopeFilter, fileSyncOptions,
                    set.metadataDirectoryPath, Settings.metaFileRem,
                    set.tempDirectoryPath, set.pathToSaveConflictLoserFiles);

                // Task curent: Skip delete
                // ChangeType:
                // Create A file or folder will be created.
                // Delete A file or folder will be deleted.
                // Update A file or folder will be updated.
                // Rename A file or folder will be renamed.
                remotePro.ApplyingChange += ProviderEvent_ApplyingChange;
                localPro.ApplyingChange += ProviderEvent_ApplyingChange;
                

                // Ask providers to detect changes
                //FileSyncProvider.DetectChanges( ) is called either implicitly 
                //by the SyncOrchestrator.Synchronize( ) method,
                //or explicitly if the FileSyncProvider options specifies
                //that DetectChanges is to be called explicitly
                localPro.DetectChanges();
                remotePro.DetectChanges();

                sourceId = localPro.ReplicaId;
                destId = remotePro.ReplicaId;

                // 2. registry
                if (reg == null)
                {
                    reg = new RegistryLocal();
                    RegistryLocal.Update(null, Settings.port, sourceId, set.dirLocal);
                }

                // Init Sync
                orchestrator = new SyncOrchestrator();
                orchestrator.LocalProvider = localPro;
                orchestrator.RemoteProvider = remotePro;
                orchestrator.Direction = way;
                //orchestrator.SessionProgress += Orchestrator_SessionProgress;
                retVal = true;
            }
            catch (Exception ex)
            {
                //("Sync fail: " + ex.ToString());

                CleanUp();

                retVal = false;

                throw ex;
            }
            //finally


            return retVal;
        }

        private Guid NewSyncGuid()
        {
            //return new SyncId(Guid.NewGuid());
            return Guid.NewGuid();
        }

        public Guid GetIdLocal()
        {
            return sourceId;
           // return localPro.ReplicaId;
        }

        public Guid GetIdRemote()
        {
            return destId;
            //return remotePro.ReplicaId;
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

        public SyncOperationStatistics Sync(string lStr, string rStr)
        {

            //if (set.ErrCount > Settings.ErrCountMax) return null;

            // 1. dir struct
            set.dirLocalSync = set.dirLocal + set.GetDirLocalSync(rStr);
            set.RefreshPaths(lStr, rStr);          

            // 3. WCF Sync
            set.optFilter = new FileSyncScopeFilter();

            set.optFilter.AttributeExcludeMask = set.excludeFileAttributes;
            foreach (string s in Settings.excludeFileExtensions)
                set.optFilter.FileNameExcludes.Add(s);

            set.directoryStructureIsOk =
                set.PrepareDirectories(set.dirLocal, set.dirRemote);

            set.remotePathIsOk = !set.dirRemote.Contains(Settings.chSlash);

            if (InitSync(set.optWay, set.optFilter, set.optFileSync))
            {
                return Sync();
            }

            return null;
        }

        private SyncOperationStatistics Sync()
        {
            if (!set.directoryStructureIsOk || !set.remotePathIsOk) return null;

            SyncOperationStatistics retVal = null;

            if (orchestrator != null)
            {
                try
                {
                    retVal = orchestrator.Synchronize();
                }
                catch(Exception ex)
                {
                    if (set.ErrCount < Settings.ErrCountMax) ++set.ErrCount;
                    throw ex;
                }
            }
            else
            {
                //("orchestrator !exists");
            }

            return retVal;
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
