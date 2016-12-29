using Microsoft.Win32;

namespace VwSyncSever
{
    class Registry
    {

        public void AlterKey()
        {
            RegistryKey key = null;
            //key= Registry.Local.OpenSubKey("Software", true);

            key.CreateSubKey("AppName");
            key = key.OpenSubKey("AppName", true);


            key.CreateSubKey("AppVersion");
            key = key.OpenSubKey("AppVersion", true);

            key.SetValue("yourkey", "yourvalue");
        }


    }
}
