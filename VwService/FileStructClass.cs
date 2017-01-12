using VwSyncSever;

namespace VwService
{
    static class FileStructClass
    {
        public static bool Start()
        {
            bool retVal = false;
            string path = RegistryLocal.GetLocalPath();
            retVal = path != null;












            if (retVal)
            {
                SerSettings.dirLocal = path;

                return true;
            }

            return false;
        }
    }
}
