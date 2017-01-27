using System;
using System.IO;

namespace VwSer
{
    public static class Lib
    {
        // ----------------- LOGGER
        //private static int logCheckDelay = 0;
        //private const int maxLogCheckDelay = 4000000; // 4 MB

        private static Func<string, int> logFunc = Log;

        public static void WrLog(Exception ex)
        {
            logFunc.BeginInvoke(DateTime.Now.ToString() + " : " + ex.Source.ToString().Trim() + " : " +
                ex.Message.ToString().Trim(), null, null);
        }

        public static void WrLog(string s)
        {
            logFunc.BeginInvoke(s, null, null);
        }

        private static int Log(string s)
        {
            //(new Thread(() =>            {

            StreamWriter sw = null;

            try
            {
                string logPathNew = SerSettings.dirLocal + "\\Log.txt";


                //++logCheckDelay;

                //if (logCheckDelay > 32)
                //{
                //    if ((new FileInfo(logPathNew)).Length > maxLogCheckDelay) // clear file
                //    {
                //        File.WriteAllText(logPathNew, string.Empty);
                //    }
                //    logCheckDelay = 0;
                //}

                sw = new StreamWriter(logPathNew, true);
                sw.WriteLine(DateTime.Now.ToString() + " : " + s);
                sw.Flush();
                sw.Close();
            }
            catch { }

            //})).Start();
            return 0;
        }

    }
}
