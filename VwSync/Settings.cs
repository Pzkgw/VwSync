﻿using System.IO;
using System.Linq;
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
            metaFileLoc = "L_filesync.metadata",
            metaFileRem = "R_filesync.metadata",
            regPathLoc = @"SOFTWARE\Wow6432Node\GTS Global Intelligence\CAVI SyncLoc",
            //regPathRem = @"SOFTWARE\Wow6432Node\GTS Global Intelligence\CAVI SyncRem",
            serNameLoc = "CAVISync",
            serLogFile = "Log.txt",
            serPasswFile = "Passwords.txt";

        public static string serExe = "VwSer.exe"; //c:\_ToDo\Sync\trunk\VwSyncSever\bin\Release\            

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

        public const char backSlash = '\\', chSlash = '@'; // caracterul ce inlocuieste slash

        public bool directoryStructureIsOk, remotePathIsOk;

        // de cate ori am avut fail de syncronizare
        public int ErrCount;
        public static readonly int ErrCountMax = 512;

        public Settings(string localDir, string remoteDir)
        {
            dirLocalSync = localDir + GetDirLocalName(remoteDir);
            RefreshPaths(localDir, remoteDir);

            optFilter = new FileSyncScopeFilter();

            optFilter.AttributeExcludeMask = excludeFileAttributes;
            foreach (string s in Settings.excludeFileExtensions)
                optFilter.FileNameExcludes.Add(s);
        }

        /// <summary>
        /// Given a password file and url as path,
        /// search the username and password required on the given path
        /// returns index for the resulted line
        /// </summary>
        /// <param name="passFile">Input password file</param>
        /// <param name="url"></param>
        /// <param name="usr"></param>
        /// <param name="pas"></param>
        public static int SearchPasswordFile(string passFile, string url, ref string usr, ref string pas)
        {
            usr = string.Empty;
            pas = string.Empty;

            if (File.Exists(passFile))
            {
                StreamReader sr = new StreamReader(passFile);
                string line = null;
                int i, retVal = 0;
                //Lib.WrLog("||FindUserDetails " + passFile + " || " + url);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(url))
                    {
                        line = line.Substring(url.Length + 1, line.Length - url.Length - 1);
                        i = line.IndexOf(',');
                        if (i > 0)
                        {
                            usr = line.Substring(0, i);
                            pas = line.Substring(i + 1, line.Length - i - 1);
                        }
                        //Lib.WrLog("||FindUserDetails " + usr + " || " + pas);
                        sr.Close();
                        return retVal;
                    }
                    //line.Substring(line.fi)
                    ++retVal;
                }
                sr.Close();
            }

            return 0; 
        }


        public static string GetDirRemoteName(string localDir) // dirRemote
        {
            if (string.IsNullOrEmpty(localDir)) return null;
            return localDir.Replace(chSlash, backSlash);
        }

        public static string GetDirLocalName(string remoteDir) // dirRemote
        {
            if (string.IsNullOrEmpty(remoteDir)) return null;

            string rFileName = null;

            rFileName = remoteDir.Replace(backSlash, chSlash);

            switch (remoteDir[1])
            {
                case ':':// Windows local prezent la inspectie
                    rFileName = rFileName.Remove(1, 1);
                    break;
                case backSlash:
                    //if (remoteDirectory[0] == Settings.backSlash) // network Windows device
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

        }

        public static bool LocalDirCheck(string dir)
        {
            string rs;
            int startIdx;

            foreach (string s in Directory.GetDirectories(dir))
            {
                startIdx = s.LastIndexOf(Settings.backSlash) + 1;
                if (startIdx > 0)
                {
                    rs = s.Substring(startIdx, s.Length - startIdx);

                    if (rs.Contains(Settings.chSlash))
                    {
                        rs = Settings.GetDirRemoteName(rs);
                        if (Utils.IsEnglishLetter(rs[0])) rs = rs.Insert(1, ":"); // director local
                        //VwSer.Lib.WrLog(rs);
                        if (Utils.IsValidPath(rs) || (Directory.Exists(rs) && Utils.DirectoryExists(rs)))
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
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

            if (!Directory.Exists(dir[1]) && !Utils.IsValidPath(dir[1]))
            {
                //("No directory for remote provider");
                return false;
            }

            //File.WriteAllText(Path.Combine(folderA, "A.txt"), " zbang ... ");

            return true;
        }


    }
}
