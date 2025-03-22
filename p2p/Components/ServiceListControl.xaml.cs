using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using p2p.Contexts;
using p2p.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using System.Net;
using System.Linq;
using Windows.Devices.Input;
using Microsoft.UI.Xaml.Data;
using p2p.Utils;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
//using CommunityToolkit.WinUI.;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace p2p.Components
{
    public sealed partial class ServiceListControl : UserControl
    {
        private readonly ApplicationContext context;
        public ServiceDiscoveryViewModel ViewModel { get; private set; }
        public ServiceListControl()
        {
            this.InitializeComponent();
            ViewModel = new ServiceDiscoveryViewModel(DispatcherQueue); 
            this.DataContext = ViewModel;
            context = App.AppContext;

        }
        private void AdvertiseService_Click(object sender, RoutedEventArgs e)
        {
            context.MdnsController.AdvertiseService();
        }
    }
    public partial class ServiceDiscoveryViewModel : ObservableObject
    {
        private readonly ApplicationContext context;

        public ObservableCollection<DeviceModel> DiscoveredServices { get; } = [];

        [ObservableProperty]
        private string connectionButtonText = "Conectar";

        public RelayCommand<DeviceModel> ConnectCommand { get; }
        public RelayCommand DiscoverCommand = new(() =>
        {
            Debug.WriteLine("DiscoverCommand executed");
            var _ = App.AppContext.WifiDirectController.StartWiFiDirectAsync();
        });

        public RelayCommand<DeviceModel> InviteCommand { get; }  =  new((deviceModel) =>
        {
            Debug.WriteLine("[InviteCommand] executed");
            if (deviceModel?.device != null)
            {
            var _ = App.AppContext.WifiDirectController.Invite(deviceModel.device);
            } else
            {
                Debug.WriteLine("[InviteCommand] device is null");

            }
        });

        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;
        public ServiceDiscoveryViewModel(Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue)
        {

            _dispatcherQueue = dispatcherQueue;
            context = App.AppContext;
            context.MdnsController.DeviceDiscovered += OnServiceReceived;
            context.WifiDirectController.DeviceDiscovered += OnServiceReceived;
            context.WifiDirectController.DeviceIpResolved += OnServiceReceived;
            context.MdnsController.StartDiscovery();
            var _ = context.WifiDirectController.StartWiFiDirectAsync();
            //IPAddress.Any
            context.SocketManager.StartServer(IPAddress.Any.ToString());


            ConnectCommand = new RelayCommand<DeviceModel>(async (param) =>
            {
                Debug.WriteLine("ConnectCommand executed");

                if (param is DeviceModel service)
                {
                    if (service?.socket?.RemoteEndPoint != null)
                    {
                        context.SocketManager.DisconnectClient(service?.socket?.RemoteEndPoint);
                    }
                    else
                    {
                        await context.SocketManager.ConnectAsync(service.Address, service.Port);
                    }

                    UpdateButtonText();
                }
            });
        }
        private void OnServiceReceived(DeviceModel service)
        {
            if (_dispatcherQueue != null)
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    var existingDevice = DiscoveredServices.FirstOrDefault(d => d.Name == service.Name);
                    if (existingDevice != null)
                    {
                        //ObjectMerger.Merge(existingDevice, service);
                        ObjectMerger.Merge(service, existingDevice);
                        int index = DiscoveredServices.IndexOf(existingDevice);
                        DiscoveredServices[index] = service;

                    }
                    else
                    {
                        DiscoveredServices.Add(service);
                    }
                });
            }
            else
            {
                Debug.WriteLine("no hay _dispatcherQueue");
            }
        }
        private void UpdateButtonText()
        {
            ConnectionButtonText = context.SocketManager.IsConnected ? "Desconectar" : "Conectar";
        }
        public void Cleanup()
        {
            context.MdnsController.DeviceDiscovered -= OnServiceReceived;
        }
    }
}
