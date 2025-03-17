using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using p2p.Controllers;

namespace p2p.Contexts
{
    public class ApplicationContext
    {
        public MdnsController MdnsController { get; private set; }
        public SocketManager SocketManager { get; private set; }

        public ApplicationContext()
        {
            MdnsController = new MdnsController();
            SocketManager = new SocketManager();

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
