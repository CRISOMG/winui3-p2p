using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using Windows.Devices.Enumeration;

namespace p2p.Models
{
    public class DeviceModel : IEquatable<DeviceModel>
    {
        public string? Name { get; set; } // Nombre del servicio
        public string? Address { get; set; } // Dirección IP
        public int Port { get; set; } // Puerto
        public string? DeviceName { get; set; } // Nombre del dispositivo
        public string? MacAddress { get; set; } // Dirección MAC
        public bool? IsConnected { get; set; } // Estado de la conexión
        public Socket? socket; // Estado de la conexión
        public DeviceInformation? device; // Estado de la conexión
        public bool? canConnect { get; set; } // Estado de la conexión

        public bool Equals(DeviceModel other)
        {
            if (other == null) return false;
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
