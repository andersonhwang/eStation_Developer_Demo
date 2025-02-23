using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Demo_Common.Helper
{
    internal class DeviceHelper
    {
        /// <summary>
        /// Get mac address
        /// </summary>
        /// <param name="adapter">Default adapter is eth0</param>
        /// <returns>Mac adress bytes</returns>
        public static PhysicalAddress GetMacAddress(string adapter = "eth0")
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            if (nics == null || nics.Length == 0)
            {
                return PhysicalAddress.None;
            }

            foreach (NetworkInterface ni in nics)
            {
                if (ni.Description != adapter) continue;
                return ni.GetPhysicalAddress();
            }
            return PhysicalAddress.None;
        }

        /// <summary>
        /// Get IP address
        /// </summary>
        /// <param name="adapter">Default adapter is eth0</param>
        /// <returns>IP adress bytes</returns>
        public static IPAddress GetIPAddress(string adapter = "eth0")
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            if (nics == null || nics.Length == 0)
            {
                return IPAddress.None;
            }

            foreach (NetworkInterface ni in nics)
            {
                if (ni.Description != adapter) continue;
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork) return ip.Address;
                }
            }
            return IPAddress.None;
        }
    }
}
