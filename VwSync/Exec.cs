
namespace VwSyncSever
{
    public static class Exec
    {
        public static bool SerIsOn()
        {
            return Services.ServiceIsInstalled(Settings.serNameLoc);
        }
        public static void SerStart()
        {            
            Services.Start(Settings.serNameLoc, 100);
            //Utils.ExecuteCommand("net start" + Settings.serName);
        }
        public static void SerStop()
        {
            //Utils.ExecuteCommand("net stop \"" + Settings.serName + "\"");
            Services.Stop(Settings.serNameLoc, 100);
        }
        public static void SerDelete()
        {
            SerDelete(SerIsOn());
        }
        public static void SerDelete(bool isOnNow)
        {
            if (isOnNow)
            {
                SerStop();
                //Utils.ExecuteCommand("sc delete \"" + Settings.serName + "\"");
                Services.Uninstall(Settings.serNameLoc);
            }
        }

    }
}
