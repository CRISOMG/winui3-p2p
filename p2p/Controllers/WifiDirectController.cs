using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Makaretu.Dns;
using p2p.Contexts;
using p2p.Models;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFiDirect;
using Windows.Networking;

namespace p2p.Controllers
{
    public class WifiDirectController
    {
        public event Action<DeviceModel> DeviceDiscovered;
        public event Action<DeviceModel> DeviceIpResolved;

        //public WifiDirectController()
        //{
        //}


        private WiFiDirectAdvertisementPublisher publisher;
        private WiFiDirectConnectionListener connectionListener;

        public void AdviseNewDevice(DeviceModel device) {
            DeviceIpResolved?.Invoke(device);
        }
        public async Task Invite(DeviceInformation  device) {
            try
            {
                // Obtener el dispositivo Wi-Fi Direct a partir de la información del dispositivo
                Debug.WriteLine($"p2p invitaiton to {device.Name} {device.Id}");
                var wifiDirectDevice = await WiFiDirectDevice.FromIdAsync(device.Id);

                if (wifiDirectDevice != null)
                {
                    // Verificar el estado de conexión
                    var accessStatus = wifiDirectDevice.ConnectionStatus;
                    if (accessStatus == WiFiDirectConnectionStatus.Connected)
                    {
                        // Obtener los pares de puntos finales de conexión
                        var endpointPairs = wifiDirectDevice.GetConnectionEndpointPairs();

                        if (endpointPairs != null && endpointPairs.Count > 0)
                        {
                            Debug.WriteLine($"Conexión exitosa a {device.Name}");
                            foreach (var endpointPair in endpointPairs)
                            {
                                Debug.WriteLine($"Endpoint: {endpointPair.RemoteHostName.RawName}");
                                var port = 8888;
                                AdviseNewDevice(new DeviceModel
                                {
                                    Name = device.Name,
                                    Address = endpointPair.RemoteHostName.RawName,
                                    Port = port,
                                    p2p_ip = $"{endpointPair.RemoteHostName.RawName}:{port}",
                                    ip = $"{endpointPair.RemoteHostName.RawName}:{port}",
                                    IsConnected = false,
                                    canConnect = true,
                                });
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"No se encontraron pares de puntos finales para {device.Name}.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Acceso denegado a {device.Name}");
                    }
                }
                else
                {
                    Debug.WriteLine("Error al obtener el dispositivo Wi-Fi Direct.");
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Debug.WriteLine($"Ocurrió un error en la invitacion del dispositivo ({device.Name}): {ex.Message}");
            }
        }
        public async Task StartWiFiDirectAsync()
        {
            // Iniciar publicidad Wi-Fi Direct
          
            var selector = WiFiDirectDevice.GetDeviceSelector();
            publisher = new WiFiDirectAdvertisementPublisher();
            connectionListener = new WiFiDirectConnectionListener();
            connectionListener.ConnectionRequested += async (sender, args) =>
            {
                Debug.WriteLine("Solicitud de conexión recibida.");
                // Obtener información del solicitante
                var connectionRequest = args.GetConnectionRequest();
                Debug.WriteLine($"Dispositivo solicitante: {connectionRequest.DeviceInformation.Name} ");
                try
                {

                    var connection = await WiFiDirectDevice.FromIdAsync(connectionRequest.DeviceInformation.Id);
                    if (connection != null)
                    {
                        Debug.WriteLine("Conexión aceptada.");
                        await HandleIncomingConnection(connection, connectionRequest.DeviceInformation);
                    }
                    else
                    {
                        Debug.WriteLine("Error al aceptar la conexión.");
                    }
                }
                catch (Exception ex) // Captura cualquier excepción
                {
                    Debug.WriteLine($"Error al aceptar la conexión: {ex.Message}");
                }


            };

            // Configurar la publicidad del servicio Wi-Fi Direct
            publisher.Advertisement.ListenStateDiscoverability = WiFiDirectAdvertisementListenStateDiscoverability.Normal;
            //publisher.Advertisement.IsAutonomousGroupOwnerEnabled = true;
            publisher.Advertisement.IsAutonomousGroupOwnerEnabled = false; // No actuar como GO automáticamente
            publisher.Advertisement.LegacySettings.IsEnabled = true; // Compatibilidad con dispositivos antiguos
            //publisher.Advertisement.LegacySettings.Ssid = "MyWiFiDirectDevice"; // Nombre de red opcional

            var watcher = DeviceInformation.CreateWatcher(selector);
            watcher.Added += (s, e) =>
            {
                Debug.WriteLine($"[watcher] Dispositivo encontrado: {e.Name}");
            };
            watcher.Start();

            var devices = await DeviceInformation.FindAllAsync(selector);
            foreach (var device in devices)
            {
                DeviceDiscovered?.Invoke(new DeviceModel
                {
                    Name = device.Name,
                    device = device,
                });
                Debug.WriteLine($"[DeviceInformation.FindAllAsync] Dispositivo encontrado: {device.Name}");

            }

            publisher.Start();

            Debug.WriteLine("Wi-Fi Direct iniciado y esperando conexiones.");
        }

        private async Task HandleIncomingConnection(WiFiDirectDevice connection, DeviceInformation deviceInfo)
        {
            //Debug.WriteLine("Conexión establecida, configurando sockets...");

            var endpointPairs = connection.GetConnectionEndpointPairs();
            if (endpointPairs.Count > 0)
            {
                var port = 8888;
                var endpoint = endpointPairs[0];
                string ipAddress = endpoint.RemoteHostName.RawName;
                var arry_address = ipAddress.Split(":");
                 Debug.WriteLine($"IP asignada por Wi-Fi Direct: {ipAddress}");
                DeviceDiscovered?.Invoke(new DeviceModel
                {
                    Name = deviceInfo.Name,
                    Address = arry_address[0],
                    Port = port,
                    canConnect = true,
                    p2p_ip = arry_address[0],
                    ip= $"{arry_address[0]}:{port}"
                });

            }
            else
            {
                Debug.WriteLine("No hay endpoint disponible.");
            }
        }
    }

}
