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
using System.Collections.Generic;
using System;
//using CommunityToolkit.WinUI.;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace p2p.Components
{
    public sealed partial class ServiceListControl : UserControl
    {
        private readonly ApplicationContext context;
        

        public RelayCommand<DeviceModel> OpenDialogCommand { get; } 
        public ServiceDiscoveryViewModel ViewModel { get; private set; }
        public ServiceListControl()
        {
            this.InitializeComponent();
            ViewModel = new ServiceDiscoveryViewModel(DispatcherQueue); 
            this.DataContext = ViewModel;
            context = App.AppContext;


            OpenDialogCommand = new(async (device) =>
            {
                Debug.WriteLine("[OpenDialogCommand] executed");
                if (device == null) return;
                ViewModel.SelectedDevice = device;
                await DeviceIpListDialog.ShowAsync();
            });

        }
        private void AdvertiseService_Click(object sender, RoutedEventArgs e)
        {
            context.MdnsController.AdvertiseService();
        }
        // M�todo que maneja la selecci�n de un dispositivo
        //private void OnDeviceSelected(DeviceModel device)
        //{
        //    ViewModel.SelectIpCommand.Execute(device);
        //}

    
        private async void ShowDeviceDialog(DeviceModel deviceInfo)
        {
            // Limpiar elementos previos
            //IpListPanel.Children.Clear();

            List<string> ipList = [deviceInfo.lan_ip, deviceInfo.p2p_ip];
            // Agregar din�micamente botones para cada IP en la lista
            foreach (var ip in ipList)
            {
                if (!string.IsNullOrEmpty(ip))
                {
                    //var button = new Button
                    //{
                    //    Content = ip,
                    //    FontSize = 20,
                    //    HorizontalAlignment = HorizontalAlignment.Stretch,
                    //    Margin = new Thickness(5)
                    //};

                    //button.Click += (_, _) =>
                    //{
                    //    //deviceInfo.Ip = ip;
                    //    //viewModel.PublishEvent(deviceInfo);
                    //    DeviceIpListDialog.Hide(); // Cierra el di�logo despu�s de seleccionar la IP
                    //};

                    //IpListPanel.Children.Add(button);
                }
            }

            // Mostrar el di�logo
            await DeviceIpListDialog.ShowAsync();
        }

    }
    public partial class ServiceDiscoveryViewModel : ObservableObject
    {
        private readonly ApplicationContext context;
        private DeviceModel _selectedDevice;
        public DeviceModel SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public ObservableCollection<DeviceModel> DiscoveredServices { get; } = [];

        [ObservableProperty]
        private string connectionButtonText = "Conectar";

        public RelayCommand<DeviceModel> ConnectCommand { get; }
        public RelayCommand DiscoverCommand { get; } 
        public RelayCommand<string> SelectIpCommand { get; }


       

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
            context.SocketManager.StartServer(IPAddress.Any.ToString());
            DiscoverCommand = new(() =>
            {
                Debug.WriteLine("DiscoverCommand executed");
                var _ = App.AppContext.WifiDirectController.StartWiFiDirectAsync();
                DiscoveredServices.Clear();
            });
            SelectIpCommand = new((param) => {
                if (SelectedDevice is DeviceModel service)
                {
                    int index = DiscoveredServices.IndexOf(SelectedDevice);
                    service.ip = param;
                    DiscoveredServices[index] = service;
                }
            });

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
                

                    if (!string.IsNullOrEmpty(service?.p2p_ip) && !service.ip_list.Contains(service.p2p_ip)) service?.ip_list?.Add(service.p2p_ip);
                    if (!string.IsNullOrEmpty(service?.lan_ip) && !service.ip_list.Contains(service.lan_ip)) service?.ip_list?.Add(service.lan_ip);
                        
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
