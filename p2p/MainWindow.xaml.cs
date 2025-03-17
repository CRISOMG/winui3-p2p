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


using Windows.Devices.WiFiDirect;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

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

            StartWiFiDirectAsync();
        }

        private WiFiDirectAdvertisementPublisher publisher;
        private WiFiDirectConnectionListener connectionListener;

        public async Task StartWiFiDirectAsync()
        {
            // Iniciar publicidad Wi-Fi Direct
            publisher = new WiFiDirectAdvertisementPublisher();
            connectionListener = new WiFiDirectConnectionListener();

            connectionListener.ConnectionRequested += async (sender, args) =>
            {
                Debug.WriteLine("Solicitud de conexión recibida.");

                // Obtener información del solicitante
                var connectionRequest = args.GetConnectionRequest();
                Debug.WriteLine($"Dispositivo solicitante: {connectionRequest.DeviceInformation.Id}");

                // Aceptar la conexión
                var connection = await WiFiDirectDevice.FromIdAsync(connectionRequest.DeviceInformation.Id);
                if (connection != null)
                {
                    Debug.WriteLine("Conexión aceptada.");
                    await HandleIncomingConnection(connection);
                }
                else
                {
                    Debug.WriteLine("Error al aceptar la conexión.");
                }
            };

            // Configurar la publicidad del servicio Wi-Fi Direct
            publisher.Advertisement.ListenStateDiscoverability = WiFiDirectAdvertisementListenStateDiscoverability.Normal;
            //publisher.Advertisement.IsAutonomousGroupOwnerEnabled = true;

            publisher.Start();
            Debug.WriteLine("Wi-Fi Direct iniciado y esperando conexiones.");
        }

        private async Task HandleIncomingConnection(WiFiDirectDevice connection)
        {
            //Debug.WriteLine("Conexión establecida, configurando sockets...");

            var endpointPairs = connection.GetConnectionEndpointPairs();
            if (endpointPairs.Count > 0)
            {
                string ipAddress = endpointPairs[0].LocalHostName.RawName;
                Debug.WriteLine($"IP asignada por Wi-Fi Direct: {ipAddress}");
                context.SocketManager.StartServer(ipAddress);
            }
            else
            {
                Debug.WriteLine("No hay endpoint disponible.");
            }
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
