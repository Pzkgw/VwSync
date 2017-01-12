using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WindowsService
{
    public static class Lib
    {

        public static void WrLog(Exception ex)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(@"c:\___\" + "\\Log.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " : " + ex.Source.ToString().Trim() + " : " +
                    ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch { }
        }


        public static void WrLog(string s)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(@"c:\___\" + "\\Log.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " : " + s);
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

    }
}
