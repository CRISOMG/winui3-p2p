using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using p2p.Controllers;

namespace p2p.Contexts
{
    public class ApplicationContext
    {
        public MdnsController MdnsController { get; private set; }
        public SocketManager SocketManager { get; private set; }
        public WifiDirectController WifiDirectController { get; private set; }

        public ApplicationContext()
        {
            SocketManager = new SocketManager();
            MdnsController = new MdnsController();
            WifiDirectController = new WifiDirectController();

            //MdnsController.ServiceDiscovered += (s, e) =>
            // {
            //     foreach (var address in e.Service.Addresses)
            //     {
            //         Debug.WriteLine($"IP descubierta por mDNS: {address}");
            //     }
            // };
        }
    }

}
