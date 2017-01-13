using System;
using System.IO;

namespace VwService
{
    public static class Lib
    {

        // ----------------- FOLOSIT DOAR PENTRU TESTE

        public static void WrLog(Exception ex)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(SerSettings.dirLocal + "\\Log.txt", true);
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
                sw = new StreamWriter(SerSettings.dirLocal + "\\Log.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + " : " + s);
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

    }
}
