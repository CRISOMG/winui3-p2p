using System;
using System.Collections.ObjectModel;
using p2p.Models;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFiDirect;

namespace p2p.Controllers
{

    //public class DeviceController(DeviceListModel deviceList)
    //{
    //    private DeviceListModel _deviceList = deviceList;


    //    public async void ScanWiFiDirectDevices()
    //    {

    //        var watcher = DeviceInformation.CreateWatcher(WiFiDirectDevice.GetDeviceSelector());
    //        watcher.Added += (s, e) =>
    //        {
    //            _deviceList.Devices.Add(new Models.DeviceModel
    //            {
    //                Name = e.Name,
    //                ConnectionType = "Wi-Fi Direct"
    //            });
    //        };
    //        watcher.Start();

    //        var wifiDirectDeviceSelector = Windows.Devices.WiFiDirect.WiFiDirectDevice.GetDeviceSelector();
    //        var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(wifiDirectDeviceSelector);

    //        foreach (var device in devices)
    //        {
    //            // Procesar el dispositivo detectado
    //            _deviceList.Devices.Add(new Models.DeviceModel
    //            {
    //                Name = device.Name,
    //                ConnectionType = "Wi-Fi Direct"
    //            });
    //        }

    //    }
    //}
}
