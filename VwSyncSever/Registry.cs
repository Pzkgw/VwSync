using System;
using System.Net;
using Microsoft.Win32;

namespace VwSyncSever
{
    class Registry
    {
        const string strCDir = "\\Clienti";

        internal void UpdateBase(IPAddress ipLocal, int portListener, string path)
        {
            RegistryKey keySv = null;

            keySv = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Settings.registryPath, true);

            if (keySv == null) // HKEY_LOCAL_MACHINE\
            {
                keySv = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(Settings.registryPath,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

                Microsoft.Win32.Registry.LocalMachine.CreateSubKey(Settings.registryPath + strCDir,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            //if (IPAddress.TryParse(ipLocal.ToString(), out ipLocalNonStr))

            keySv.SetValue("IPLocal", ipLocal);
            //keySv.SetValue("portListener", "portListener");
            keySv.SetValue("Path", path);
            keySv.Close();
        }

        internal void UpdateDeriv(int idClient, int port, string path)
        {
            RegistryKey keyCl = null;

            string keyStr = Settings.registryPath + strCDir + path;

            keyCl = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyStr, true);

            if (keyCl == null)
            {
                keyCl = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(keyStr,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            keyCl.SetValue("SyncTime", DateTime.Now.ToString());

            keyCl.Close();
        }


    }
}
