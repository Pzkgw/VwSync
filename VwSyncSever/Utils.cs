using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace VwSyncSever
{
    class Utils
    {

        /// <summary>
        /// Get all files from a directory, exluding cerain extensions
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="excludedExtensions"></param>
        /// <returns></returns>
        internal static List<string> GetFilesAndDirectories(String directory, params string[] excludedExtensions)
        {
            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();
            stack.Push(directory);

            while (stack.Count > 0)
            {
                string temp = stack.Pop();

                try
                {
                    result.AddRange(
                        Directory.GetFiles(temp, "*.*").Where(s => !(s.EndsWith('.' + excludedExtensions[0]))));//

                    foreach (string directoryName in
                      Directory.GetDirectories(temp))
                    {
                        stack.Push(directoryName);
                    }
                }
                catch
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
        internal static string GetLocalIpAddress()
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

                    return address.Address.ToString();
                }
            }

            return mostSuitableIp != null
                ? mostSuitableIp.Address.ToString()
                : "";
        }


    }
}
