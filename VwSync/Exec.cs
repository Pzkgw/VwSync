
namespace VwSyncSever
{
    public static class Exec
    {
        public static void SerStop()
        {
            Utils.ExecuteCommand("net stop \"" + Settings.serName + "\"");
        }
        public static void SerDelete()
        {
            SerStop();
            Utils.ExecuteCommand("sc delete \"" + Settings.serName + "\"");
        }

        public static void Sync()
        {
            Orchestrator o;
            o = new Orchestrator(new Settings(@"c:\___\", @"c:\__###\SDL1\"));

            //SyncOperationStatistics stats =
            o.Sync(o.set.dirLocal, o.set.dirRemote);
        }
    }
}
