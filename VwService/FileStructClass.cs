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

            retVal = (SerSettings.dirLocal != null &&
                Directory.Exists(SerSettings.dirLocal));

            if (retVal)
            {
                string rs;
                int startIdx, count = 0;

                Orchestrator o;
                SyncOperationStatistics stats;                

                foreach (string s in Directory.GetDirectories(SerSettings.dirLocal))
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
                                
                                o = new Orchestrator(new Settings(SerSettings.dirLocal, rs));

                                stats = o.Sync(SerSettings.dirLocal, rs);

                                if (SerSettings.debug)
                                {
                                    Lib.WrLog(string.Format(" Sync done at {0} in {1}ms", rs, stats.SyncEndTime.Subtract(stats.SyncStartTime).Milliseconds));
                                    /*
                                    Lib.WrLog(string.Format(
                                        " Task done in {0}ms.  Download changes total:{1}  Download changes applied:{2}  Download changes failed:{3}, UploadChangesTotal: {4}",
                                        stats.SyncEndTime.Subtract(stats.SyncStartTime).Milliseconds,
                                        stats.DownloadChangesTotal, stats.DownloadChangesApplied, stats.DownloadChangesFailed,
                                        stats.UploadChangesTotal)); */   
                                }

                                if (!SerSettings.run) return false;
                            }
                        }
                    }
                }

                retVal = count > 0;
            }

            SerSettings.run = false;
            return retVal;
        }
    }
}
