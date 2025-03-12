using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using p2p.Models;
using p2p.Controllers;

using Windows.Networking.Proximity;
using Windows.Foundation;
using System;



namespace p2p
{
    public sealed partial class MainWindow : Window
    {
        private DeviceListModel deviceList = new DeviceListModel();
        private DeviceController deviceController;

        public MainWindow()
        {
            this.InitializeComponent();
            //this.DataContext = deviceList;
            deviceController = new DeviceController(deviceList);
        }

        private void ScanDevices(object sender, RoutedEventArgs e)
        {
            deviceController.ScanWiFiDirectDevices();

            // Asigna el origen de elementos del ListView directamente
            DeviceListView.ItemsSource = deviceList.Devices;

        }

        private MdnsController mdnsController = new MdnsController();

        private void StartDiscovery_Click(object sender, RoutedEventArgs e)
        {
            mdnsController.StartDiscovery();
        }

        private void AdvertiseService_Click(object sender, RoutedEventArgs e)
        {
            mdnsController.AdvertiseService("192.168.1.100", 8080);
        }
    }
}
