using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VwSyncSever
{
    static class Verify
    {

        internal static bool LocalDirCheck(string dir)
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
                        rs = rs.Replace(Settings.chSlash, Settings.backSlash);
                        if (Utils.IsEnglishLetter(rs[0])) rs = rs.Insert(1, ":"); // director local
                        VwSer.Lib.WrLog(rs);
                        if (Utils.IsValidPath(rs) || (Directory.Exists(rs) && Utils.DirectoryExists(rs)))
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }



    }
}
