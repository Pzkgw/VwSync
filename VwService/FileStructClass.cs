using System.IO;
using System.Linq;
using Microsoft.Synchronization;
using VwSyncSever;

namespace VwService
{
    public static class FileStructClass
    {
        public static bool RunSync()
        {
            SerSettings.run = true;

            bool retVal = false;

            string path = SerSettings.dirLocal;// RegistryLocal.GetLocalPath();
            //path SerSettings.dirLocal = @"c:\_sync\";

            retVal = (path != null &&
                Directory.Exists(path));

            if (retVal)
            {
                string rs;
                int startIdx, count = 0;

                Orchestrator o;
                SyncOperationStatistics stats;                

                foreach (string s in Directory.GetDirectories(path))
                {
                    startIdx = s.LastIndexOf('\\') + 1;
                    if (startIdx > 0)
                    {
                        rs = s.Substring(startIdx, s.Length - startIdx);

                        if(rs.Contains(Settings.chSlash))
                        {
                            rs = rs.Replace(Settings.chSlash, '\\').Insert(1, ":");
                            if(Directory.Exists(rs) && Utils.DirectoryExists(rs))
                            {
                                ++count;
                                
                                o = new Orchestrator(new Settings(path, rs));

                                stats = o.Sync(path, rs);

                                Lib.WrLog(" L: " + path + " ||| R: " + rs);

                                if (!SerSettings.run) return false;
                            }
                        }
                    }
                }

                retVal = count > 0;
            }

            if(retVal)
            {
                SerSettings.dirLocal = path;
            }

            SerSettings.run = false;
            return retVal;
        }
    }
}
