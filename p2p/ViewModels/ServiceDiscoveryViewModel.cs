using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using p2p.Contexts;
using p2p.Controllers;
using p2p.Models;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace p2p.ViewModels
{

    public partial class ServiceDiscoveryViewModel : ObservableObject
    {
        private readonly ApplicationContext context;

        public ObservableCollection<DiscoveredService> DiscoveredServices { get; } = [];

        [ObservableProperty]
        private string connectionButtonText = "Conectar";

        public RelayCommand<DiscoveredService> ConnectCommand { get; }
        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        public ServiceDiscoveryViewModel(Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue)
        {

            _dispatcherQueue = dispatcherQueue;
            context = App.AppContext;
            context.MdnsController.ServiceResolved += OnServiceReceived;
            context.MdnsController.StartDiscovery();
         
            ConnectCommand = new RelayCommand<DiscoveredService>(async (param) =>
            {
                Debug.WriteLine("ConnectCommand executed");

                if (param is DiscoveredService service)
                {
                    if (context.SocketManager.IsConnected)
                    {
                        context.SocketManager.Disconnect();
                    }
                    else
                    {
                        await context.SocketManager.ConnectAsync(service.Address, service.Port);
                    }

                    UpdateButtonText();
                }
            });
        }

        private void OnServiceReceived(DiscoveredService service)
        {
            if (_dispatcherQueue != null)
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    DiscoveredServices.Add(service);
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
            context.MdnsController.ServiceResolved -= OnServiceReceived;
        }
    }
}
