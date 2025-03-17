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
namespace p2p.Controllers
{
    public class DiscoveredService : IEquatable<DiscoveredService>
    {
        public string Name { get; set; } // Nombre del servicio
        public string Address { get; set; } // Dirección IP
        public int Port { get; set; } // Puerto
        public string DeviceName { get; set; } // Nombre del dispositivo
        public string MacAddress { get; set; } // Dirección MAC
        public bool IsConnected { get; set; } // Estado de la conexión

        public bool Equals(DiscoveredService other)
        {
            if (other == null) return false;
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
    public class MdnsController
    {
        private MulticastService mdns;
        private ServiceDiscovery discovery;

        private HashSet<DiscoveredService> discoveredServicesHashSet = new HashSet<DiscoveredService>();

        string serviceName = "MyOMGAppService-Windows";
        string serviceType = "_myomgservice._tcp";
        ushort port = 8888;
        public event Action<DiscoveredService> ServiceResolved;
        public void StartDiscovery()
        {
            mdns = new MulticastService();
            mdns.Start();
            mdns.AnswerReceived += (s, message) =>
            {
                var aRecord = message.Message?.Answers.OfType<ARecord>().FirstOrDefault();
                if (aRecord != null)
                {
                    Debug.WriteLine("[mdns.AnswerReceived]");
                    // Obtiene el registro SRV que corresponde al mensaje actual
                    var srvRecord = message.Message.Answers.OfType<SRVRecord>().FirstOrDefault();
                    int port = (int)(srvRecord?.Port ?? 0);
                    if (port <= 0)
                    {
                        Debug.WriteLine("Error: el puerto no es válido.");
                        return; // Evita intentar conectarse si el puerto es inválido
                    }

                    var service = new DiscoveredService
                    {
                        Name = srvRecord.Target.ToString(),
                        Address = aRecord.Address.ToString(),
                        Port = port,
                        // Aquí puedes añadir más información si está disponible
                    };
                    if (!discoveredServicesHashSet.Contains(service)) { 
                        discoveredServicesHashSet.Add(service);
                        Debug.WriteLine($"🌐 Service URI: {service.Address}:{service.Port}");
                        ServiceResolved?.Invoke(service);
                    }
                }
            };

            discovery = new ServiceDiscovery(mdns);
            discovery.ServiceInstanceDiscovered += (s, args) =>
            {
                 // Obtener el registro SRV (contiene el hostname del servicio)
                var srvRecord = args.Message.Answers.OfType<SRVRecord>().FirstOrDefault();
                if (srvRecord != null)
                {
                    // Enviar consulta manual para obtener la IP del hostname
                    var newService = new DiscoveredService { Name = srvRecord.Target.ToString() };
                    if (!discoveredServicesHashSet.Contains(newService))
                    {
                        Debug.WriteLine($"[ServiceInstanceDiscovered] \n 🔍 Servicio: {args.ServiceInstanceName} Hostname: {srvRecord.Target}, Puerto: {srvRecord.Port}");
                        mdns.SendQuery(srvRecord.Target);
                        Debug.WriteLine($" Enviando consulta para {srvRecord.Target}");
                    }
                    else
                    {
                        Debug.WriteLine($"[ServiceInstanceDiscovered] El servicio {srvRecord.Target} ya fue descubierto anteriormente.");
                    }
                }

            };
           
            //discovery.QueryAllServices();
            discovery.QueryServiceInstances(serviceType);
            Debug.WriteLine("📡 Iniciando descubrimiento mDNS...");
        }

        public List<DiscoveredService> GetDiscoveredServices()
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
