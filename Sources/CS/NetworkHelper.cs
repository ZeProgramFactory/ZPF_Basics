using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;  // add ref: System.Net
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ZPF
{
   /// <summary>
   /// 
   /// </summary>
   public static class NetworkHelper
   {
      /// <summary>
      /// Indicates whether any network connection is available
      /// Filter connections below a specified speed, as well as virtual network cards.
      /// </summary>
      /// <returns>
      ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsNetworkAvailable()
      {
         return IsNetworkAvailable(0);
      }

      /// <summary>
      /// Indicates whether any network connection is available.
      /// Filter connections below a specified speed, as well as virtual network cards.
      /// </summary>
      /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
      /// <returns>
      ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
      /// </returns>
      public static bool IsNetworkAvailable(long minimumSpeed)
      {
         if (!NetworkInterface.GetIsNetworkAvailable())
            return false;

         foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
         {
            // filter so we see only Internet adapters
            if (ni.OperationalStatus == OperationalStatus.Up)
            {
               if ((ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
                   (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback))
               {
                  IPv4InterfaceStatistics statistics = ni.GetIPv4Statistics();

                  // all testing seems to prove that once an interface
                  // comes online it has already accrued statistics for
                  // both received and sent...

                  if ((statistics.BytesReceived > 0) && (statistics.BytesSent > 0))
                  {
                     return true;
                  }
               }
            }

            //// discard because of standard reasons
            //if ((ni.OperationalStatus != OperationalStatus.Up) ||
            //    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
            //    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
            //   continue;

            //// this allow to filter modems, serial, etc.
            //// I use 10000000 as a minimum speed for most cases
            //if (ni.Speed < minimumSpeed)
            //   continue;

            //// discard virtual cards (virtual box, virtual pc, etc.)
            //if ((ni.Description.IndexOf("Hyper-V Virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
            //    (ni.Name.IndexOf("Hyper-V Virtual", StringComparison.OrdinalIgnoreCase) >= 0))
            //   continue;

            //// discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
            //if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
            //   continue;

            //return true;
         };

         return false;
      }

      public static bool IsInternetAvailable()
      {
         try
         {
            Dns.GetHostEntry("www.google.com"); //using System.Net;
            return true;
         }
         catch 
         {
            return false;
         }
      }

      //private List<IPAddress> _ipAddresses = new List<IPAddress>();

      //public NetworkConnectivity()
      //{
      //   _ipAddresses = new List<IPAddress>();
      //}

      //#region Public Properties
      //public int CountIPAddresses
      //{
      //   get { return this.IPAddresses.Count; }
      //}
      //public List<IPAddress> IPAddresses
      //{
      //   get
      //   {
      //      _ipAddresses.Clear();
      //      // Get a listing of all network adapters
      //      NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
      //      foreach (NetworkInterface adapter in adapters)
      //      {
      //         IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
      //         GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
      //         // If this adapter has at least 1 IPAddress
      //         if (addresses.Count > 0)
      //         {
      //            // Loop through all IP Addresses
      //            foreach (GatewayIPAddressInformation address in addresses)
      //            {
      //               _ipAddresses.Add(address.Address);
      //            }
      //         }
      //      }
      //      return _ipAddresses;
      //   }
      //}
      //public bool IsInternetConnected
      //{
      //   get
      //   {
      //      if (this.CountIPAddresses == 0)
      //      {
      //         return false;
      //      }
      //      else
      //      {
      //         //IPAddress[] ips = ResolveDNSAddress("google.com");
      //         //return PingIPAddressPool(ips);
      //         return PingIPAddress("72.14.204.104"); // Google IP
      //      }
      //   }
      //}
      //#endregion

      //#region Public Methods
      //public IPAddress[] ResolveDNSAddress(string UrlAddress)
      //{
      //   IPHostEntry hostInfo = Dns.Resolve(UrlAddress);
      //   return hostInfo.AddressList;
      //}
      //public bool PingIPAddressPool(IPAddress[] ipAddresses)
      //{
      //   foreach (IPAddress ip in ipAddresses)
      //   {
      //      if (PingIPAddress(ip.Address.ToString()))
      //      {
      //         return true;
      //      }
      //   }
      //   return false;
      //}
      //public bool PingIPAddress(string ip)
      //{
      //   // Pinging
      //   IPAddress addr = IPAddress.Parse(ip);
      //   Ping pingSender = new Ping();
      //   PingOptions options = new PingOptions();

      //   // Use the default Ttl value which is 128,
      //   // but change the fragmentation behavior.
      //   options.DontFragment = true;

      //   // Create a buffer of 32 bytes of data to be transmitted.
      //   string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
      //   byte[] buffer = Encoding.ASCII.GetBytes(data);
      //   int timeout = 15; // seconds to wait for response
      //   int attempts = 2; // ping attempts
      //   for (int i = 0; i < attempts; i++)
      //   {
      //      PingReply reply = pingSender.Send(addr, timeout, buffer, options);
      //      if (reply.Status == IPStatus.Success)
      //      { return true; }
      //   }
      //   return false;
      //}
      //#endregion


      ///// <summary>
      ///// 
      ///// </summary>
      ///// <returns></returns>
      //public static bool IsInternetAccessAvailable()
      //{
      //   //ToDo: test if network ...

      //   //if (NetworkInformation.GetInternetConnectionProfile() != null)
      //   //{
      //   //   switch (NetworkInformation.GetInternetConnectionProfile().GetNetworkConnectivityLevel())
      //   //   {
      //   //      case NetworkConnectivityLevel.InternetAccess:
      //   //         return true;

      //   //      default:
      //   //         return false;
      //   //   }
      //   //}
      //   //else
      //   {
      //      return false;
      //   };
      //}
   }
}
