

using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace VwSync
{
    public class Imperson
    {


        // obtains user token
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public static IntPtr userHandle;

        public static string mesaj;
        public static bool DoWorkUnderImpersonation(string path)
        {
            //elevate privileges before doing file copy to handle domain security
            WindowsImpersonationContext impersonationContext = null;
            userHandle = IntPtr.Zero;
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;
            string domain = "GI"; // ConfigurationManager.AppSettings[
            string user = "bogdan.visoiu";
            string password = "Parola32167"; 

            try
            {
                mesaj = (("windows identify before impersonation: " + WindowsIdentity.GetCurrent().Name));

                // if domain name was blank, assume local machine
                if (domain == "")
                    domain = Environment.MachineName;

                // Call LogonUser to get a token for the user
                bool loggedOn = LogonUser(user,
                                            domain,
                                            password,
                                            LOGON32_LOGON_INTERACTIVE,
                                            LOGON32_PROVIDER_DEFAULT,
                                            ref userHandle);

                if (!loggedOn)
                {
                    mesaj = ("Exception impersonating user, error code: " + Marshal.GetLastWin32Error());
                    return false;
                }

                // Begin impersonating the user
                impersonationContext = WindowsIdentity.Impersonate(userHandle);

                mesaj = ("Main() windows identify after impersonation: " + WindowsIdentity.GetCurrent().Name);

                //run the program with elevated privileges (like file copying from a domain server)
                return DoWork(path);

            }
            catch (Exception ex)
            {
                mesaj = ("Exception impersonating user: " + ex.Message);
            }
            finally
            {
                // Clean up
                if (impersonationContext != null)
                {
                    impersonationContext.Undo();
                }

                if (userHandle != IntPtr.Zero)
                {
                    CloseHandle(userHandle);
                }
            }

            return false;
        }


        private static bool DoWork(string path)
        {
            //everything in here has elevated privileges

            //example access files on a network share through e$ 
            //string[] files = System.IO.Directory.GetFiles(@"\\domainserver\e$\images", "*.jpg");
            if (path[1] == ':') return true;

            bool rv = false;

            using (WindowsImpersonationContext context = WindowsIdentity.Impersonate(userHandle))
            {
                rv = (Directory.Exists(@"\\10.10.10.47\video\"));//  && VwSyncSever.Utils.DirectoryExists(@"\\10.10.10.47\video\gi test\")
                context.Undo();
            }

            return rv;
        }


    }
}
