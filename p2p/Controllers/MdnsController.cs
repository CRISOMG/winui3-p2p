using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using System.Diagnostics;


namespace p2p.Controllers
{
    class MdnsController
    {
        private DatagramSocket socket;
        private const string MdnsAddress = "224.0.0.251";
        private const string MdnsPort = "5353";

        public async void StartDiscovery()
        {
            try
            {
                socket = new DatagramSocket();
                socket.MessageReceived += OnMessageReceived;

                await socket.BindServiceNameAsync("8080");
                socket.JoinMulticastGroup(new HostName(MdnsAddress));

                    
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en descubrimiento mDNS: {ex.Message}");
            }
        }

        private async void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                DataReader reader = args.GetDataReader();
                byte[] buffer = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(buffer);

                string message = Encoding.UTF8.GetString(buffer);
                Debug.WriteLine($"📡 Mensaje mDNS recibido: {message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al recibir mensaje: {ex.Message}");
            }
        }

        public async void AdvertiseService( string ipAddress, int port)
        {
            try
            {
                IOutputStream outputStream = await socket.GetOutputStreamAsync(new HostName(MdnsAddress), MdnsPort);
                DataWriter writer = new DataWriter(outputStream);

                string serviceMessage = $"_myomgapp._tcp";
                byte[] data = Encoding.UTF8.GetBytes(serviceMessage);

                writer.WriteBytes(data);
                await writer.StoreAsync();

                Debug.WriteLine($"📢 Servicio anunciado: {serviceMessage}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error al anunciar servicio: {ex.Message}");
            }
        }
    }
}
