using System;

namespace VwSer
{

    public static class SerSettings
    {
        // = @"c:\_sync\";
        public static string
            dirLocal,
            logPath,
            passwFilePath; // 

        public static int errCount;
        public static int errCountMax = 1024;
        public static bool run, logPathInit;

        public static void UpdateFileLocations(char backSlash, string serLogFile, string serPasswFile)
        {
            logPath = string.Format("{0}{1}{2}", dirLocal, backSlash, serLogFile);
            passwFilePath = string.Format("{0}{1}{2}", dirLocal, backSlash, serPasswFile);
        }
    }

}
