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

            keySv = Registry.LocalMachine.OpenSubKey(Settings.regLocalPath, false); // open to just read
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

        public static void Update(IPAddress ip, int port, Guid id, string path)
        {
            RegistryKey keySv = null;

            keySv = Registry.LocalMachine.OpenSubKey(Settings.regLocalPath, true);

            if (keySv == null) // HKEY_LOCAL_MACHINE\
            {
                keySv = Registry.LocalMachine.CreateSubKey(Settings.regLocalPath,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

                //Microsoft.Win32.Registry.LocalMachine.CreateSubKey(Settings.registryPath + strCDir,
                //    RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            //if (IPAddress.TryParse(ipLocal.ToString(), out ipLocalNonStr))

            if (ip != null) keySv.SetValue("ip", ip);
            if (port > 0) keySv.SetValue("port", port);
            if (id != Guid.Empty) keySv.SetValue("ID", id);
            if (path != null) keySv.SetValue("Path", path);

            keySv.Close();
        }

        public static string GetLocalPath()
        {
            RegistryKey keySv = null;

            keySv = Registry.LocalMachine.OpenSubKey(Settings.regLocalPath, false);

            if (keySv != null)
            {
                object o = null;
                o = keySv.GetValue("Path");

                if (o != null) return o.ToString();
            }
            return null;
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
