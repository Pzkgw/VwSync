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

                // file sync providers
                localPro = new FileSyncProvider(set.dirLocalSync, scopeFilter, fileSyncOptions,
                    set.metadataDirectoryPath, Settings.metaLocalFile,
                    set.tempDirectoryPath, set.pathToSaveConflictLoserFiles);

                remotePro = new FileSyncProvider(set.dirRemote, scopeFilter, fileSyncOptions,
                    set.metadataDirectoryPath, Settings.metaRemoteFile,
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
                localPro.DetectChanges();
                remotePro.DetectChanges();

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

            // 2. registry
            if (reg == null)
            {
                reg = new RegistryLocal();
                reg.UpdateBase(null, set.dirLocal);
            }            

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
