using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Evil.Util
{
    public class IpAddr
    {
        /// <summary>
        /// 获取本机所有IP
        /// 不包括回环地址
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIps()
        {
            var ips = new List<IPAddress>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        var address = ip.Address;
                        // 是回环地址
                        if (IPAddress.IsLoopback(address)) 
                            continue;
                        ips.Add(address);
                    }
                }
            }
            return ips;
        }

        public static IPAddress? GetLocalIpv4()
        {
            var ips = GetLocalIps();
            return ips.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}