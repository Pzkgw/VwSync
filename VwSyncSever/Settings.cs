using System.IO;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;

namespace VwSyncSever
{

    /// <summary>
    /// Denumiri, directoare
    /// </summary>
    static class Settings
    {
        internal static string
            dirLocal = @"c:\___\",
            dirRemote = @"c:\__###\SDL1\",
            dirLocalSync;
        //@"\\CJ-PC\Users\Default\AppData",
        //@"\\10.10.10.47\video\gi test\demo\";
        //@"c:\_ToDo\TestHik\TestHik\bin\x86\Debug\DbgMessages\";

        internal static string metadataDirectoryPath, tempDirectoryPath, pathToSaveConflictLoserFiles;

        internal const string
            dirForMetadata = "\\___meta___",
            dirForTemporaryFiles = "\\___temp___",
            dirForConflictedFiles = "\\___conf___",
            metaLocalFile = "L_filesync.metadata",
            metaRemoteFile = "R_filesync.metadata";

        internal static FileAttributes excludeFileAttributes =
            FileAttributes.System | FileAttributes.Hidden;

        internal static FileSyncOptions optFileSync = FileSyncOptions.None;
        internal static SyncDirectionOrder optWay = SyncDirectionOrder.Download;
        internal static FileSyncScopeFilter optFilter;

        internal static string[] excludeFileExtensions =
            new string[] { "*.tmp", "*.lnk", "*.pst" };

        internal static bool directoryStructureIsOk;

        internal static string GetDirLocalSync()
        {
            string rFileName = null;

            rFileName = dirRemote.Replace('\\', '$');

            switch (dirRemote[1])
            {
                case ':':// Windows local prezent la inspectie
                    rFileName = rFileName.Remove(1, 1);
                    break;
                case '\\':
                    //if (remoteDirectory[0] == '\\') // network Windows device
                    // rFileName = rFileName;
                    break;
                default:
                    break;
            }

            return rFileName;
        }

        internal static void SetupDirectoryStruct()
        {
            if (!Directory.Exists(dirLocal)) Directory.CreateDirectory(dirLocal);
            if (!Directory.Exists(dirLocalSync)) Directory.CreateDirectory(dirLocalSync);

            metadataDirectoryPath = dirLocalSync + dirForMetadata; /* "filesync.metadata" */
            tempDirectoryPath = dirLocalSync + dirForTemporaryFiles;
            pathToSaveConflictLoserFiles = dirLocalSync + dirForConflictedFiles;

            if (!Directory.Exists(metadataDirectoryPath)) Directory.CreateDirectory(metadataDirectoryPath);
            if (!Directory.Exists(tempDirectoryPath)) Directory.CreateDirectory(tempDirectoryPath);
            if (!Directory.Exists(pathToSaveConflictLoserFiles)) Directory.CreateDirectory(pathToSaveConflictLoserFiles);

        }


        /// <summary>
        /// Prepare directories for sync
        /// </summary>
        internal static bool PrepareDirectories(params string[] dir)
        {
            if (dir == null || dir[1] == null)
            {
                //("No directories to sync ...");
                return false;
            }

            if (!Directory.Exists(dir[0]))
            {
                //Show("No directory for local provider"); return false;
                Directory.CreateDirectory(dir[0]);
            }

            if (!Directory.Exists(dir[1]))
            {
                //("No directory for remote provider");
                return false;
            }

            //File.WriteAllText(Path.Combine(folderA, "A.txt"), " zbang ... ");

            return true;
        }


    }
}
