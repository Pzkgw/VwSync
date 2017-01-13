
namespace VwSyncSever
{
    public static class Exec
    {
        public static void SerStop()
        {            
            //Utils.ExecuteCommand("net stop \"" + Settings.serName + "\"");
            Services.Stop(Settings.serName, 1000);
        }
        public static void SerDelete()
        {
            if (Services.ServiceIsInstalled(Settings.serName))
            {
                SerStop();
                //Utils.ExecuteCommand("sc delete \"" + Settings.serName + "\"");
                Services.Uninstall(Settings.serName);
            }
        }

    }
}
