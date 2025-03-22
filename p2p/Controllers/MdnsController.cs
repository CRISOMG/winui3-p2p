using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using System.Diagnostics;
using Makaretu.Dns;
using Windows.Networking.Connectivity;
using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;
using System.Net;
using p2p.Models;
namespace p2p.Controllers
{

    public class MdnsController
    {
        private MulticastService mdns;
        private ServiceDiscovery discovery;

        private HashSet<DeviceModel> discoveredServicesHashSet = [];

        string serviceName = "MyOMGAppService-Windows";
        string serviceType = "_myomgservice._tcp";
        ushort port = 8888;

        public event Action<DeviceModel> DeviceDiscovered;

        //public event Action<ServiceInstanceDiscoveryEventArgs> _ServiceInstanceDiscovered;
        public void StartDiscovery()
        {

            DeviceModel newService = null;


            mdns = new MulticastService();
            mdns.Start();
            mdns.AnswerReceived += (s, message) =>
            {
                Debug.WriteLine("[AnswerReceived]");
                var records = new
                {
                    srvRecord = message.Message.Answers.OfType<SRVRecord>().FirstOrDefault(),
                    aRecord = message.Message?.Answers.OfType<ARecord>().FirstOrDefault(),
                    txtRecord = message.Message?.Answers.OfType<TXTRecord>().FirstOrDefault(),
                };
            };

            discovery = new ServiceDiscovery(mdns);
            discovery.ServiceInstanceDiscovered += (s, args) =>
            {
                Debug.WriteLine("[ServiceInstanceDiscovered]");

                var srvRecord = args.Message.Answers.OfType<SRVRecord>().FirstOrDefault();
                var aRecord = args.Message?.Answers.OfType<ARecord>().FirstOrDefault();
                var txtRecord = args.Message.Answers.OfType<TXTRecord>().FirstOrDefault();

                if (srvRecord != null && aRecord != null && txtRecord != null)
                {
                    //mdns.SendQuery(srvRecord.Target);
                    string deviceName = null;
                    foreach (var entry in txtRecord.Strings)
                    {
                        var parts = entry.Split('=');
                        if (parts.Length == 2 && parts[0] == "deviceName")
                        {
                            deviceName = parts[1];
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(deviceName))
                    {
                        Debug.WriteLine("❌ No se encontró el 'deviceName' en los TXT Records");
                        return;
                    }

                    int port = (int)(srvRecord.Port);
                    if (port <= 0)
                    {
                        Debug.WriteLine($"❌ Puerto inválido: {port}");
                        return;
                    }
                    newService = new DeviceModel
                    {
                        Name = deviceName,
                        Address = aRecord.Address.ToString(),
                        Port = port,
                        ip = $"{aRecord.Address}:{port}",
                        lan_ip = $"{aRecord.Address}:{port}"

                    };

                    if (discoveredServicesHashSet.Any(d => d.Address == newService.Address && d.Port == newService.Port))
                    {
                        Debug.WriteLine($"🔍 El servicio {newService.Name} ya ha sido descubierto.");
                        //return;
                    }

                    discoveredServicesHashSet.Add(newService);
                    Debug.WriteLine($"🌐 Nuevo servicio descubierto: {newService.Address}:{newService.Port}");
                    DeviceDiscovered?.Invoke(newService);


                }
                else
                {
                    Debug.WriteLine($"ServiceInstanceDiscovered srvRecord, aRecord y txtRecord en null");

                }

            };

            //discovery.QueryAllServices();
            discovery.QueryServiceInstances(serviceType);
            Debug.WriteLine("📡 Iniciando descubrimiento mDNS...");
        }

        public List<DeviceModel> GetDiscoveredServices()
        {
            return discoveredServicesHashSet.ToList(); // Retornar la lista de servicios descubiertos
        }

        public void AdvertiseService()
        {
            var service = new ServiceProfile(serviceName, serviceType, port);
            //service.AddProperty("info", "Mi servicio en UWP");

            discovery.Advertise(service);
            Debug.WriteLine("📢 Servicio anunciado con éxito.");
        }

        public void StopDiscovery()
        {
            discovery?.Dispose();
            mdns?.Stop();
            Debug.WriteLine("🛑 Descubrimiento detenido.");
        }
    }
}
