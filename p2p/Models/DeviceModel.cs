using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
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
        public string? ip { get; set; }
        public string? p2p_ip { get; set; }
        public string? lan_ip { get; set; }
        public List<string?>? ip_list { get; set; } = [];
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

    public partial class DeviceModelV2 : ObservableObject, IEquatable<DeviceModelV2>
    {
        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? address;

        [ObservableProperty]
        private int port;

        [ObservableProperty]
        private string? deviceName;

        [ObservableProperty]
        private string? macAddress;

        [ObservableProperty]
        private string? ip;

        [ObservableProperty]
        private string? p2pIp;

        [ObservableProperty]
        private string? lanIp;

        [ObservableProperty]
        private bool? isConnected;

        [ObservableProperty]
        private bool? canConnect;

        public List<string> IpList { get; set; } = ["test", "test"];

        public Socket? Socket { get; set; }
        public DeviceInformation? Device { get; set; }

        public bool Equals(DeviceModelV2? other)
        {
            if (other == null) return false;
            return string.Equals(name, other.name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name);
        }
    }
}
