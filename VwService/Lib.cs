using System;
using System.IO;

namespace VwService
{
    public static class Lib
    {

        // ----------------- LOGGER
        private static int logCheckDelay = 0;
        private const int maxLogCheckDelay = 2000000; // 2 MB
        public static void WrLog(Exception ex)
        {
            WrLog(DateTime.Now.ToString() + " : " + ex.Source.ToString().Trim() + " : " +
                ex.Message.ToString().Trim());
        }


        public static void WrLog(string s)
        {

            StreamWriter sw = null;

            try
            {
                string logPathNew = SerSettings.dirLocal + "\\Log.txt";

                /*
                ++logCheckDelay;

                if (logCheckDelay > 32)
                {
                    if ((new FileInfo(logPathNew)).Length > maxLogCheckDelay) // clear file
                    {
                        File.WriteAllText(logPathNew, string.Empty);
                    }
                    logCheckDelay = 0;
                }*/

                sw = new StreamWriter(logPathNew, true);
                sw.WriteLine(DateTime.Now.ToString() + " : " + s);
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

    }
}
