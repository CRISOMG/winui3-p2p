using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using p2p.Models;
using p2p.Controllers;

using Windows.Networking.Proximity;
using Windows.Foundation;
using System;
using p2p.Contexts;
using Makaretu.Dns;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using p2p.ViewModels;


namespace p2p
{
 
    public sealed partial class MainWindow : Window
    {
        private DeviceListModel deviceList = new DeviceListModel();
        private DeviceController deviceController;

        private readonly ApplicationContext context;

        public MainWindow()
        {
            this.InitializeComponent();
            context = App.AppContext;
            deviceController = new DeviceController(deviceList);
        }
        private void StartDiscovery_Click(object sender, RoutedEventArgs e)
        {
            context.MdnsController.StartDiscovery();
        }

        private void AdvertiseService_Click(object sender, RoutedEventArgs e)
        {
            context.MdnsController.AdvertiseService();
        }
    }
}
