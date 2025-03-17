using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using p2p.Contexts;
using p2p.Controllers;
using System.Collections.ObjectModel;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace p2p.Controls
{
    public sealed partial class ServiceListControl : UserControl
    {

        public ServiceDiscoveryViewModel ViewModel { get; private set; }
        public ServiceListControl()
        {
            this.InitializeComponent();
            ViewModel = new ServiceDiscoveryViewModel(DispatcherQueue); 
            this.DataContext = ViewModel;
        }
    }
    public partial class ServiceDiscoveryViewModel : ObservableObject
    {
        private readonly ApplicationContext context;

        public ObservableCollection<DiscoveredServiceModel> DiscoveredServices { get; } = [];

        [ObservableProperty]
        private string connectionButtonText = "Conectar";

        public RelayCommand<DiscoveredServiceModel> ConnectCommand { get; }
        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;
        public ServiceDiscoveryViewModel(Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue)
        {

            _dispatcherQueue = dispatcherQueue;
            context = App.AppContext;
            context.MdnsController.ServiceResolved += OnServiceReceived;
            context.MdnsController.StartDiscovery();

            ConnectCommand = new RelayCommand<DiscoveredServiceModel>(async (param) =>
            {
                Debug.WriteLine("ConnectCommand executed");

                if (param is DiscoveredServiceModel service)
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
        private void OnServiceReceived(DiscoveredServiceModel service)
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
