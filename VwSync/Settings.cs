using System.IO;
using System.Net;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;

namespace VwSyncSever
{

    /// <summary>
    /// Denumiri, directoare
    /// </summary>
    public class Settings
    {
        public IPAddress IPServer;

        public const string
            dirForMetadata = "\\___meta___",
            dirForTemporaryFiles = "\\___temp___",
            dirForConflictedFiles = "\\___conf___",
            metaLocalFile = "L_filesync.metadata",
            metaRemoteFile = "R_filesync.metadata",
            registryPath = @"SOFTWARE\Wow6432Node\GTS Global Intelligence\CAVI Sync",
            serName = "CAVISync",
            serDesc = "File syncronization service";

        public static string serExecutabil = "VwService.exe"; //c:\_ToDo\Sync\trunk\VwSyncSever\bin\Release\
            

        public string
            dirLocal, dirRemote, dirLocalSync,
            metadataDirectoryPath, tempDirectoryPath, pathToSaveConflictLoserFiles;
        //@"\\CJ-PC\Users\Default\AppData",
        //@"\\10.10.10.47\video\gi test\demo\";
        //@"c:\_ToDo\TestHik\TestHik\bin\x86\Debug\DbgMessages\";




        public FileAttributes excludeFileAttributes =
            FileAttributes.System | FileAttributes.Hidden;

        public FileSyncOptions optFileSync = FileSyncOptions.None;
        public SyncDirectionOrder optWay = SyncDirectionOrder.Download;
        public FileSyncScopeFilter optFilter;

        public static string[] excludeFileExtensions =
            new string[] { "*.tmp", "*.lnk", "*.pst" };

        public static char chSlash = '$'; // caracterul ce inlocuieste slash

        public bool directoryStructureIsOk;

        public Settings(string localDir, string remoteDir)
        {
            dirLocalSync = localDir + GetDirLocalSync(remoteDir);
            RefreshPaths(localDir, remoteDir);
        }

        public string GetDirLocalSync(string remoteDir) // dirRemote
        {
            string rFileName = null;

            rFileName = remoteDir.Replace('\\', chSlash);

            switch (remoteDir[1])
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


        public void RefreshPaths(string localDir, string remoteDir)
        {
            dirLocal = localDir;
            dirRemote = remoteDir;

            if (!dirLocal.EndsWith("\\")) dirLocal = dirLocal + "\\";
            if (!dirRemote.EndsWith("\\")) dirRemote = dirRemote + "\\";
            if (!dirLocalSync.EndsWith("\\")) dirLocalSync = dirLocalSync + "\\";
        }

        public void SetupDirectoryStruct()
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
        public bool PrepareDirectories(params string[] dir)
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
