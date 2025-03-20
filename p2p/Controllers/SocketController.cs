using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class SocketManager : ObservableObject
{
    //private readonly ApplicationContext context;
    public Socket socket;
    private Socket listenerSocket;
    public event Action<string> MessageReceived;
    public event Action<Socket> OnNewConnection;
    public event Action OnDisconnected;

    [ObservableProperty]
    private bool _isConnected;

    private Dictionary<EndPoint, Socket> connectedSockets = new();

    public IReadOnlyCollection<Socket> ConnectedClients => connectedSockets.Values;

    public void StartServer(string ipAddress, int port = 8888)
    {
        try
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            listenerSocket.Listen(10); // Máximo de 10 conexiones en cola
            Debug.WriteLine($"Servidor escuchando en el puerto {port}...");

            // Iniciar la aceptación de conexiones de manera asíncrona
            AcceptConnectionsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al iniciar el servidor: {ex.Message}");
        }
    }

    private async void AcceptConnectionsAsync()
    {
        while (true)
        {
            try
            {
                // Aceptar una nueva conexión
                Socket clientSocket = await listenerSocket.AcceptAsync();
                Debug.WriteLine("Nueva conexión aceptada.");
                connectedSockets[clientSocket.RemoteEndPoint] = clientSocket;
                OnNewConnection?.Invoke(clientSocket);

                // Iniciar la recepción de mensajes desde el cliente
                StartReceiving(clientSocket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al aceptar la conexión: {ex.Message}");
                break;
            }
        }
    }


    public async Task ConnectAsync(string ipAddress, int port)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(ipAddress, port);
            connectedSockets[socket.RemoteEndPoint] = socket;
            Debug.WriteLine("Conexión establecida.");
            StartReceiving(socket);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error de conexión: {ex.Message}");
        }
    }

 
    private async void StartReceiving(Socket socket)
    {
        Debug.WriteLine($"Escuchando desde {socket.RemoteEndPoint}");
        byte[] buffer = new byte[1024];

        while (socket.Connected)
        {
            try
            {
                int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(message);
                }
                else
                {
                    Debug.WriteLine($"Cliente {socket.RemoteEndPoint} cerró la conexión.");
                    HandleDisconnection(socket);
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error de recepción en {socket.RemoteEndPoint}: {ex.Message}");
                HandleDisconnection(socket);
                break;
            }
        }
    }


    public void SendMessage(EndPoint recipient, string message)
    {
        if (connectedSockets.TryGetValue(recipient, out var socket) && socket.Connected)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            socket.Send(data);
            Debug.WriteLine($"Mensaje enviado a {recipient}: {message}");
        }
        else
        {
            Debug.WriteLine($"El socket para {recipient} no está conectado.");
        }
    }

    private void HandleDisconnection(Socket socket)
    {
        if (socket != null)
        {
            Debug.WriteLine($"Cliente {socket.RemoteEndPoint} desconectado.");
            socket.Close();
            connectedSockets.Remove(socket.RemoteEndPoint);
            OnDisconnected?.Invoke();
        }
    }

    public void DisconnectClient(EndPoint clientEndpoint)
    {
        if (connectedSockets.TryGetValue(clientEndpoint, out var socket))
        {
            HandleDisconnection(socket);
        }
    }

    public void DisconnectAll()
    {
        foreach (var socket in connectedSockets.Values)
        {
            socket.Close();
        }
        connectedSockets.Clear();
    }
}
