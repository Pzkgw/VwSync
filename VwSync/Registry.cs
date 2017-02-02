using System;
using System.Net;
using Microsoft.Win32;

namespace VwSyncSever
{

    public class RegistryCon
    {
        public static string GetSyncPath()
        {
            RegistryKey keySv = null;

            object retVal = null;

            keySv = Registry.LocalMachine.OpenSubKey(Settings.regPathLoc, false); // open to just read
            if (keySv == null)
            {
                retVal = keySv.GetValue("Path");
            }

            keySv.Close();

            return ((retVal == null) ? null : retVal.ToString());
        }

        //const string strCDir = "\\Clienti";

        public static void Update(IPAddress ip, int port, Guid guid, string path)
        {
            RegistryKey keySv = null;

            keySv = Registry.LocalMachine.OpenSubKey(Settings.regPathLoc, true);

            if (keySv == null) // HKEY_LOCAL_MACHINE\
            {
                keySv = Registry.LocalMachine.CreateSubKey(Settings.regPathLoc,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

                //Microsoft.Win32.Registry.LocalMachine.CreateSubKey(Settings.registryPath + strCDir,
                //    RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            //if (IPAddress.TryParse(ipLocal.ToString(), out ipLocalNonStr))

            if (ip != null) keySv.SetValue("Ip", ip);
            if (port > 0) keySv.SetValue("Port", port);
            if (guid != Guid.Empty) keySv.SetValue("Guid", guid);
            if (path != null) keySv.SetValue("Path", path);

            keySv.Close();
        }

        public static string GetLocalPath()
        {
            RegistryKey keySv = null;

            keySv = Registry.LocalMachine.OpenSubKey(Settings.regPathLoc, false);

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
