using System;
using System.IO;
using VwSyncSever;

namespace VwSer
{
    public static class Lib
    {
        // ----------------- LOGGER
        private static int logCheckDelay = 0;
        private const int maxLogCheckDelay = 4000000; // 4 MB

        // private static Func<string, int> logFunc = Log;

        public static void WrLog(Exception ex)
        {
            //logFunc.BeginInvoke(string.Format("EXCEPTION: {0} : {1}",
            //  DateTime.Now.ToString(), ex.Message.ToString().Trim()), null, null);
            WrLog(ex.Message.ToString().Trim());
        }

        public static void WrLog(string s)
        {
            //ThreadPool.QueueUserWorkItem(_=>Log(s));
            //logFunc.BeginInvoke(s, null, null);
            Log(s);
        }

        private static int Log(string s)
        {
            //(new Thread(() =>            {

            

            try
            {
                if (!SerSettings.logPathInit)
                {
                    SerSettings.UpdateFileLocations(Settings.backSlash,
                        Settings.serLogFile, Settings.serPasswFile);

                    SerSettings.logPathInit = true;
                }

                ++logCheckDelay;

                if (logCheckDelay > 32)
                {
                    if ((new FileInfo(SerSettings.logPath)).Length > maxLogCheckDelay) 
                    {
                        File.WriteAllText(SerSettings.logPath, string.Empty);// clear file
                    }
                    logCheckDelay = 0;
                }
                using (StreamWriter sw = new StreamWriter(SerSettings.logPath, true))
                {
                    sw.WriteLine(string.Format("{0} : {1}", DateTime.Now.ToString(), s));
                    sw.Flush();
                    sw.Close();
                }
            }
            catch { }

            //})).Start();
            return 0;
        }

    }
}
