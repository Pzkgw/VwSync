using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace VwSyncSever
{
    public class Utils
    {

        /// <summary>
        /// Get all files from a directory, exluding cerain extensions
        /// First excluded extensions is for dir, next for files
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="excludedExtensions"></param>
        /// <returns></returns>
        public static List<string> GetFiles(String directory, string excludeDirNameStart, params string[] excludedExtensions)
        {

            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();
            stack.Push(directory);

            while (stack.Count > 0)
            {
                string temp = stack.Pop();

                try
                {
                    // 'Where' -> fara anumite fisiere
                    result.AddRange(
                        Directory.GetFiles(temp).Where(s => !(
                        s.EndsWith(excludedExtensions[0].Substring(2)) ||
                        s.EndsWith(excludedExtensions[1].Substring(2)) ||
                        s.EndsWith(excludedExtensions[2].Substring(2)))));

                    foreach (string directoryName in
                      Directory.GetDirectories(temp))
                    {
                        // 'if' - > fara anumite directoare
                        if (directoryName.Length > 2 &&
                            directoryName[directoryName.Length - 1] != excludeDirNameStart[0] &&
                            directoryName[directoryName.Length - 2] != excludeDirNameStart[1] &&
                            directoryName[directoryName.Length - 3] != excludeDirNameStart[2])
                            stack.Push(directoryName);
                    }
                }
                catch //(Exception ex)
                {
                    //throw new Exception("Error retrieving file or directory.");
                    return null;
                }
            }

            return result;
        }




        /// <summary>
        /// As intented, get local IP
        /// </summary>
        /// <returns>Local IP</returns>
        public static IPAddress GetLocalIpAddress()
        {
            UnicastIPAddressInformation mostSuitableIp = null;

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;

                var properties = network.GetIPProperties();

                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    if (!address.IsDnsEligible)
                    {
                        if (mostSuitableIp == null)
                            mostSuitableIp = address;
                        continue;
                    }

                    // The best IP is the IP got from DHCP server
                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    {
                        if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                            mostSuitableIp = address;
                        continue;
                    }

                    return address.Address;
                }
            }

            return mostSuitableIp != null
                ? mostSuitableIp.Address
                : null;
        }


        public static bool DirectoryExists(string path)
        {
            try
            {
                Directory.GetAccessControl(path);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// Executa comanda in Command Prompt
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteCommand(string command)
        {

            int ExitCode;
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;

            Process = Process.Start(ProcessInfo);
            Process.WaitForExit();

            ExitCode = Process.ExitCode;
            Process.Close();

            //MessageBox.Show("ExitCode: " + ExitCode.ToString(), "ExecuteCommand");
        }







        /*
                #region StringCompress


                /// <summary>
                /// Compresses the string.
                /// </summary>
                /// <param name="text">The text.</param>
                /// <returns></returns>
                public static string CompressString(string text)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(text);
                    var memoryStream = new MemoryStream();
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    {
                        gZipStream.Write(buffer, 0, buffer.Length);
                    }

                    memoryStream.Position = 0;

                    var compressedData = new byte[memoryStream.Length];
                    memoryStream.Read(compressedData, 0, compressedData.Length);

                    var gZipBuffer = new byte[compressedData.Length + 4];
                    Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                    Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                    return Convert.ToBase64String(gZipBuffer);
                }

                /// <summary>
                /// Decompresses the string.
                /// </summary>
                /// <param name="compressedText">The compressed text.</param>
                /// <returns></returns>
                public static string DecompressString(string compressedText)
                {
                    byte[] gZipBuffer = Convert.FromBase64String(compressedText);
                    using (var memoryStream = new MemoryStream())
                    {
                        int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                        memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                        var buffer = new byte[dataLength];

                        memoryStream.Position = 0;
                        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                        {
                            gZipStream.Read(buffer, 0, buffer.Length);
                        }

                        return Encoding.UTF8.GetString(buffer);
                    }
                }


                #endregion StringCompress
                */
    }



    public class NetworkDrive
    {
        public enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }

        public enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }

        public enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        }

        public enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

        public int MapNetworkDrive(string unc, string drive, string user, string password)
        {
            NETRESOURCE myNetResource = new NETRESOURCE();
            myNetResource.lpLocalName = drive;
            myNetResource.lpRemoteName = unc;
            myNetResource.lpProvider = null;
            int result = WNetAddConnection2(myNetResource, password, user, 0);
            return result;
        }
    }


    public class DriveSettings
    {
        public enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }
        public enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }
        public enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010
        }
        public enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }
        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope dwScope;
            public ResourceType dwType;
            public ResourceDisplayType dwDisplayType;
            public ResourceUsage dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }
        [DllImport("mpr.dll")]
        public static extern int WNetAddConnection2
            (ref NetResource oNetworkResource, string sPassword,
            string sUserName, int iFlags);

        [DllImport("mpr.dll")]
        public static extern int WNetCancelConnection2
            (string sLocalName, uint iFlags, int iForce);

        /* Uses Struct rather than class version of NETRESOURCE */
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection3(IntPtr hWndOwner,
            ref NetResource lpNetResource, string lpPassword,
            string lpUserName, int dwFlags);

        public static void MapNetworkDrive(string sDriveLetter, string sNetworkPath, string user, string pass)
        {
            NetResource oNetworkResource;
            oNetworkResource = new NetResource();
            oNetworkResource.dwType = ResourceType.RESOURCETYPE_DISK;
            oNetworkResource.lpLocalName = sDriveLetter;
            oNetworkResource.lpRemoteName = sNetworkPath;

            //If Drive is already mapped disconnect the current 
            //mapping before adding the new mapping
            if (IsDriveMapped(sDriveLetter))
            {
                DisconnectNetworkDrive(sDriveLetter, true);
            }

            WNetAddConnection2(ref oNetworkResource, user, pass, 0);


        }

        public static int DisconnectNetworkDrive(string sDriveLetter, bool bForceDisconnect)
        {
            if (bForceDisconnect)
            {
                return WNetCancelConnection2(sDriveLetter, 0, 1);
            }
            else
            {
                return WNetCancelConnection2(sDriveLetter, 0, 0);
            }
        }

        public static bool IsDriveMapped(string sDriveLetter)
        {
            foreach(string s in Environment.GetLogicalDrives())
            {
                if (s == sDriveLetter) return true;
            }
            return false;
        }
    }



    public class Services
    {

        #region "Environment Variables"
        public static string GetEnvironment(string name, bool ExpandVariables = true)
        {
            if (ExpandVariables)
            {
                return Environment.GetEnvironmentVariable(name);
            }
            else
            {
                return (string)Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment\").GetValue(name, "", Microsoft.Win32.RegistryValueOptions.DoNotExpandEnvironmentNames);
            }
        }

        public static void SetEnvironment(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value);
        }
        #endregion

        #region "ServiceCalls Native"
        public static ServiceController[] List { get { return ServiceController.GetServices(); } }

        //The following method tries to start a service specified by a service name. 
        //Then it waits until the service is running or a timeout occurs.
        public static void Start(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running,
                    TimeSpan.FromMilliseconds(timeoutMilliseconds));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Stop(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                // ...
            }
        }

        public static void Restart(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }

        public static bool IsInstalled(string serviceName)
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName)
                    return true;
            }
            return false;
        }
        #endregion

        #region "ServiceCalls API"
        private const int
            STANDARD_RIGHTS_REQUIRED = 0xF0000,
            SERVICE_WIN32_OWN_PROCESS = 0x00000010,
            SERVICE_CONFIG_DESCRIPTION = 0x01,
            SERVICE_WIN32_SHARE_PROCESS = 0x00000020,
            SERVICE_USER_OWN_PROCESS = 0x00000050,
            SERVICE_INTERACTIVE_PROCESS = 0x00000100;

        [Flags]
        public enum ServiceManagerRights
        {
            Connect = 0x0001,
            CreateService = 0x0002,
            EnumerateService = 0x0004,
            Lock = 0x0008,
            QueryLockStatus = 0x0010,
            ModifyBootConfig = 0x0020,
            StandardRightsRequired = 0xF0000,
            AllAccess = (StandardRightsRequired | Connect | CreateService |
            EnumerateService | Lock | QueryLockStatus | ModifyBootConfig)
        }

        [Flags]
        public enum ServiceRights
        {
            QueryConfig = 0x1,
            ChangeConfig = 0x2,
            QueryStatus = 0x4,
            EnumerateDependants = 0x8,
            Start = 0x10,
            Stop = 0x20,
            PauseContinue = 0x40,
            Interrogate = 0x80,
            UserDefinedControl = 0x100,
            Delete = 0x00010000,
            StandardRightsRequired = 0xF0000,
            AllAccess = (StandardRightsRequired | QueryConfig | ChangeConfig |
            QueryStatus | EnumerateDependants | Start | Stop | PauseContinue |
            Interrogate | UserDefinedControl)
        }

        public enum ServiceBootFlag
        {
            Start = 0x00000000,
            SystemStart = 0x00000001,
            AutoStart = 0x00000002,
            DemandStart = 0x00000003,
            Disabled = 0x00000004
        }

        public enum ServiceState
        {
            Unknown = -1, // The state cannot be (has not been) retrieved.
            NotFound = 0, // The service is not known on the host server.
            Stop = 1, // The service is NET stopped.
            Run = 2, // The service is NET started.
            Stopping = 3,
            Starting = 4,
        }

        public enum ServiceControl
        {
            Stop = 0x00000001,
            Pause = 0x00000002,
            Continue = 0x00000003,
            Interrogate = 0x00000004,
            Shutdown = 0x00000005,
            ParamChange = 0x00000006,
            NetBindAdd = 0x00000007,
            NetBindRemove = 0x00000008,
            NetBindEnable = 0x00000009,
            NetBindDisable = 0x0000000A
        }

        public enum ServiceError
        {
            Ignore = 0x00000000,
            Normal = 0x00000001,
            Severe = 0x00000002,
            Critical = 0x00000003
        }

        [StructLayout(LayoutKind.Sequential)]
        private class SERVICE_STATUS
        {
            public int dwServiceType = 0;
            public ServiceState dwCurrentState = 0;
            public int dwControlsAccepted = 0;
            public int dwWin32ExitCode = 0;
            public int dwServiceSpecificExitCode = 0;
            public int dwCheckPoint = 0;
            public int dwWaitHint = 0;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig2(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_DESCRIPTION lpInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SERVICE_DESCRIPTION
        {
            public string lpDescription;
        }

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerA")]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, ServiceManagerRights dwDesiredAccess);
        [DllImport("advapi32.dll", EntryPoint = "OpenServiceA", CharSet = CharSet.Ansi)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceRights dwDesiredAccess);
        [DllImport("advapi32.dll", EntryPoint = "CreateServiceA")]
        private static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceRights dwDesiredAccess, int dwServiceType, ServiceBootFlag dwStartType, ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);
        [DllImport("advapi32.dll")]
        private static extern int CloseServiceHandle(IntPtr hSCObject);
        [DllImport("advapi32.dll")]
        private static extern int QueryServiceStatus(IntPtr hService, SERVICE_STATUS lpServiceStatus);
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int DeleteService(IntPtr hService);
        [DllImport("advapi32.dll")]
        private static extern int ControlService(IntPtr hService, ServiceControl dwControl, SERVICE_STATUS lpServiceStatus);
        [DllImport("advapi32.dll", EntryPoint = "StartServiceA")]
        private static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

        /// <summary>
        /// Takes a service name and tries to stop and then uninstall the windows serviceError
        /// </summary>
        /// <param name="ServiceName">The windows service name to uninstall</param>
        public static void Uninstall(string ServiceName)
        {
            IntPtr scman = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr service = OpenService(scman, ServiceName, ServiceRights.StandardRightsRequired | ServiceRights.Stop | ServiceRights.QueryStatus);
                if (service == IntPtr.Zero)
                {
                    throw new ApplicationException("Service not installed.");
                }
                try
                {
                    StopService(service);
                    int ret = DeleteService(service);
                    if (ret == 0)
                    {
                        int error = Marshal.GetLastWin32Error();
                        throw new ApplicationException("Could not delete service " + error);
                    }
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scman);
            }
        }

        public static void SetDescriereServiciu(string ServiceName, string txt)
        {

            IntPtr scman = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr service = OpenService(scman, ServiceName, ServiceRights.AllAccess);
                if (service == IntPtr.Zero)
                {
                    throw new ApplicationException("Service not installed.");
                }
                try
                {
                    var pinfo = new SERVICE_DESCRIPTION
                    {
                        lpDescription = txt
                    };

                    if (!ChangeServiceConfig2(service, SERVICE_CONFIG_DESCRIPTION, ref pinfo))
                    {
                        //int error = Marshal.GetLastWin32Error();
                        //throw new ApplicationException("Could not delete service " + error);
                    }
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scman);
            }

        }


        /// <summary>
        /// Accepts a service name and returns true if the service with that service name exists
        /// </summary>
        /// <param name="ServiceName">The service name that we will check for existence</param>
        /// <returns>True if that service exists false otherwise</returns>
        public static bool ServiceIsInstalled(string ServiceName)
        {
            IntPtr scman = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr service = OpenService(scman, ServiceName,
                ServiceRights.QueryStatus);
                if (service == IntPtr.Zero) return false;
                CloseServiceHandle(service);
                return true;
            }
            finally
            {
                CloseServiceHandle(scman);
            }
        }

        /// <summary>
        /// Takes a service name, a service display name and the path to the service executable and installs / starts the windows service.
        /// </summary>
        /// <param name="ServiceName">The service name that this service will have</param>
        /// <param name="DisplayName">The display name that this service will have</param>
        /// <param name="FileName">The path to the executable of the service</param>
        public static bool Install(string ServiceName, string DisplayName,
        string FileName)
        {
            bool retVal = false;
            IntPtr scman = OpenSCManager(ServiceManagerRights.AllAccess);
            // ServiceManagerRights.Connect |  ServiceManagerRights.CreateService
            try
            {
                IntPtr service = OpenService(scman, ServiceName,
                ServiceRights.AllAccess);
                if (service == IntPtr.Zero)
                {
                    service = CreateService(scman, ServiceName, DisplayName,
                    ServiceRights.AllAccess, SERVICE_WIN32_OWN_PROCESS | SERVICE_INTERACTIVE_PROCESS,
                    ServiceBootFlag.DemandStart, ServiceError.Critical, FileName, null, IntPtr.Zero,
                    null, null, null);              //"GI", "1qaz@WSX"    // "GI\\bogdan.visoiu", "rets"  
                }
                if (service == IntPtr.Zero)
                {
                    throw new ApplicationException("Failed to install service.");
                }
                try
                {
                    //StartService(service);
                    retVal = true;
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scman);
            }

            return retVal;
        }

        /// <summary>
        /// Takes a service name and starts it
        /// </summary>
        /// <param name="Name">The service name</param>
        public static void StartService(string Name)
        {
            IntPtr scman = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr hService = OpenService(scman, Name, ServiceRights.QueryStatus |
                ServiceRights.Start);
                if (hService == IntPtr.Zero)
                {
                    throw new ApplicationException("Could not open service.");
                }
                try
                {
                    StartService(hService);
                }
                finally
                {
                    CloseServiceHandle(hService);
                }
            }
            finally
            {
                CloseServiceHandle(scman);
            }
        }

        /// <summary>
        /// Stops the provided windows service
        /// </summary>
        /// <param name="Name">The service name that will be stopped</param>
        public static void StopService(string Name)
        {
            IntPtr scman = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr hService = OpenService(scman, Name, ServiceRights.QueryStatus |
                ServiceRights.Stop);
                if (hService == IntPtr.Zero)
                {
                    throw new ApplicationException("Could not open service.");
                }
                try
                {
                    StopService(hService);
                }
                finally
                {
                    CloseServiceHandle(hService);
                }
            }
            finally
            {
                CloseServiceHandle(scman);
            }
        }

        /// <summary>
        /// Stars the provided windows service
        /// </summary>
        /// <param name="hService">The handle to the windows service</param>
        private static void StartService(IntPtr hService)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();
            StartService(hService, 0, 0);
            WaitForServiceStatus(hService, ServiceState.Starting, ServiceState.Run);
        }

        /// <summary>
        /// Stops the provided windows service
        /// </summary>
        /// <param name="hService">The handle to the windows service</param>
        private static void StopService(IntPtr hService)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();
            ControlService(hService, ServiceControl.Stop, status);
            WaitForServiceStatus(hService, ServiceState.Stopping, ServiceState.Stop);
        }

        /// <summary>
        /// Takes a service name and returns the <code>ServiceState</code> of the corresponding service
        /// </summary>
        /// <param name="ServiceName">The service name that we will check for his <code>ServiceState</code></param>
        /// <returns>The ServiceState of the service we wanted to check</returns>
        public static ServiceState GetServiceStatus(string ServiceName)
        {
            IntPtr scman = OpenSCManager(ServiceManagerRights.Connect);
            try
            {
                IntPtr hService = OpenService(scman, ServiceName,
                ServiceRights.QueryStatus);
                if (hService == IntPtr.Zero)
                {
                    return ServiceState.NotFound;
                }
                try
                {
                    return GetServiceStatus(hService);
                }
                finally
                {
                    CloseServiceHandle(scman);
                }
            }
            finally
            {
                CloseServiceHandle(scman);
            }
        }

        /// <summary>
        /// Gets the service state by using the handle of the provided windows service
        /// </summary>
        /// <param name="hService">The handle to the service</param>
        /// <returns>The <code>ServiceState</code> of the service</returns>
        private static ServiceState GetServiceStatus(IntPtr hService)
        {
            SERVICE_STATUS ssStatus = new SERVICE_STATUS();
            if (QueryServiceStatus(hService, ssStatus) == 0)
            {
                throw new ApplicationException("Failed to query service status.");
            }
            return ssStatus.dwCurrentState;
        }

        /// <summary>
        /// Returns true when the service status has been changes from wait status to desired status
        /// ,this method waits around 10 seconds for this operation.
        /// </summary>
        /// <param name="hService">The handle to the service</param>
        /// <param name="WaitStatus">The current state of the service</param>
        /// <param name="DesiredStatus">The desired state of the service</param>
        /// <returns>bool if the service has successfully changed states within the allowed timeline</returns>
        private static bool WaitForServiceStatus(IntPtr hService, ServiceState
        WaitStatus, ServiceState DesiredStatus)
        {
            SERVICE_STATUS ssStatus = new SERVICE_STATUS();
            int dwOldCheckPoint;
            int dwStartTickCount;

            QueryServiceStatus(hService, ssStatus);
            if (ssStatus.dwCurrentState == DesiredStatus) return true;
            dwStartTickCount = Environment.TickCount;
            dwOldCheckPoint = ssStatus.dwCheckPoint;

            while (ssStatus.dwCurrentState == WaitStatus)
            {
                // Do not wait longer than the wait hint. A good interval is
                // one tenth the wait hint, but no less than 1 second and no
                // more than 10 seconds.

                int dwWaitTime = ssStatus.dwWaitHint / 10;

                if (dwWaitTime < 1000) dwWaitTime = 1000;
                else if (dwWaitTime > 10000) dwWaitTime = 10000;

                System.Threading.Thread.Sleep(dwWaitTime);

                // Check the status again.

                if (QueryServiceStatus(hService, ssStatus) == 0) break;

                if (ssStatus.dwCheckPoint > dwOldCheckPoint)
                {
                    // The service is making progress.
                    dwStartTickCount = Environment.TickCount;
                    dwOldCheckPoint = ssStatus.dwCheckPoint;
                }
                else
                {
                    if (Environment.TickCount - dwStartTickCount > ssStatus.dwWaitHint)
                    {
                        // No progress made within the wait hint
                        break;
                    }
                }
            }
            return (ssStatus.dwCurrentState == DesiredStatus);
        }

        /// <summary>
        /// Opens the service manager
        /// </summary>
        /// <param name="Rights">The service manager rights</param>
        /// <returns>the handle to the service manager</returns>
        private static IntPtr OpenSCManager(ServiceManagerRights Rights)
        {
            IntPtr scman = OpenSCManager(null, null, Rights);
            if (scman == IntPtr.Zero)
            {
                throw new ApplicationException("Could not connect to service control manager.");
            }
            return scman;
        }

        #endregion

    }











}
