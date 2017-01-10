using System;
using System.Net;
using Microsoft.Win32;

namespace VwSyncSever
{

    class RegistryService
    {
        public static string GetSyncPath()
        {
            RegistryKey keySv = null;

            object retVal = null;

            keySv = Registry.LocalMachine.OpenSubKey(Settings.registryPath, false); // open to just read
            if (keySv == null)
            {
                retVal = keySv.GetValue("Path");
            }

            keySv.Close();

            return ((retVal == null) ? null : retVal.ToString());
        }
    }
    public class RegistryLocal
    {
        //const string strCDir = "\\Clienti";

        public void UpdateBase(IPAddress ipLocal, int portListener, string path)
        {
            RegistryKey keySv = null;

            keySv = Registry.LocalMachine.OpenSubKey(Settings.registryPath, true);

            if (keySv == null) // HKEY_LOCAL_MACHINE\
            {
                keySv = Registry.LocalMachine.CreateSubKey(Settings.registryPath,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

                //Microsoft.Win32.Registry.LocalMachine.CreateSubKey(Settings.registryPath + strCDir,
                //    RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            //if (IPAddress.TryParse(ipLocal.ToString(), out ipLocalNonStr))

            keySv.SetValue("IPLocal", ipLocal);
            //keySv.SetValue("portListener", "portListener");
            keySv.SetValue("Path", path);
            keySv.Close();
        }

        /*
        public void UpdateDeriv(int idClient, int port, string path)
        {
            RegistryKey keyCl = null;

            string keyStr = Settings.registryPath + strCDir + path;

            keyCl = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyStr, true);

            if (keyCl == null)
            {
                keyCl = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(keyStr,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            if (keyCl.Name.Length > 254)
            {
                System.Windows.MessageBox.Show("Name too long");
            }
            else
            {
                keyCl.SetValue("SyncTime", DateTime.Now.ToString());
            }

            keyCl.Close();
        }
        */

    }
}
