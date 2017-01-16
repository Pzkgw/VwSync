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
                startIdx = s.LastIndexOf('\\') + 1;
                if (startIdx > 0)
                {
                    rs = s.Substring(startIdx, s.Length - startIdx);

                    if (rs.Contains(Settings.chSlash))
                    {
                        rs = rs.Replace(Settings.chSlash, '\\');
                        if (rs[0] != '\\') rs = rs.Insert(1, ":"); // director local
                        if (Directory.Exists(rs) && Utils.DirectoryExists(rs))
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
