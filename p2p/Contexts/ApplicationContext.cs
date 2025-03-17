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
         
            MdnsController.ServiceResolved += async (DiscoveredService service) => {
                Debug.WriteLine("[onServiceResolved]");

                
                //await SocketManager.ConnectAsync(service.Address, service.Port);
                //if (SocketManager.socket.Connected) // Verifica si la conexión fue exitosa
                //{
                //    Debug.WriteLine("Intentando enviar un mensaje");
                //    SocketManager.SendMessage("Hola desde Windows");
                //}
                //else
                //{
                //    Debug.WriteLine("No se pudo enviar el mensaje porque no hay conexión.");
                //}

            };
        }
    }

}
