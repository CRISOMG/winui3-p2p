using System.Collections.ObjectModel;

namespace p2p.Models
{
    public class DeviceModel
    {
        public string Name { get; set; }
        public string ConnectionType { get; set; } // WiFi Direct, Bluetooth, USB
    }
}
