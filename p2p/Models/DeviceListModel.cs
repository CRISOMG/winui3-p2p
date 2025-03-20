using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p2p.Models
{
    public class DeviceListModel
    {
        public ObservableCollection<DeviceModel> Devices { get; set; } = [];
    }
}
