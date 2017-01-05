using Microsoft.Win32;

namespace VwSyncSever
{
    class Registry
    {

        RegistryKey keySv, keyCl;

        public void AlterKey()
        {
            
            keySv = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Settings.registryPath);

            if (keySv == null) // HKEY_LOCAL_MACHINE\
            {
                keySv = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(Settings.registryPath,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

                Microsoft.Win32.Registry.LocalMachine.CreateSubKey(Settings.registryPath + "\\Clienti",
                    RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            else
            {

            }

            //keySv.SetValue("Name", "Isabella");

            keySv.Close();

            /*
            RegistryKey key = null;
            //key= Registry.Local.OpenSubKey("Software", true);

            key.CreateSubKey("AppName");
            key = key.OpenSubKey("AppName", true);


            key.CreateSubKey("AppVersion");
            key = key.OpenSubKey("AppVersion", true);

            key.SetValue("yourkey", "yourvalue");*/
        }


    }
}
