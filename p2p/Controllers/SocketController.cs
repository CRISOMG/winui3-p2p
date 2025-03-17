using System;
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

    public void StartServer(string ipAddress, int port = 8888)
    {
        try
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port)); // Escucha en cualquier dirección IP
            listenerSocket.Listen(); // Máximo de 10 conexiones en cola
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
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(ipAddress, port);
            Debug.WriteLine("Conexión establecida.");
            IsConnected = true;
            StartReceiving(socket);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error de conexión: {ex.Message}");
        }
    }
    private void HandleDisconnection()
    {
        Debug.WriteLine("Desconectado del servidor.");
        socket?.Close();
        socket = null;
        IsConnected = false;
        OnDisconnected?.Invoke();
    }
    private async void StartReceiving(Socket socket)
    {

        Debug.WriteLine($"AddressFamily {socket.AddressFamily.ToString()}");

        byte[] buffer = new byte[1024];
        while (IsConnected)
        {
            try
            {
                Debug.WriteLine($"IsConnected: {IsConnected}");

                int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(message);
                }
                else
                {
                    Debug.WriteLine("Servidor cerró la conexión.");
                    HandleDisconnection();
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error de recepción: {ex.Message}");
                HandleDisconnection();
                break;
            }
        }
    }


    public void SendMessage(string message)
    {
        if (IsConnected)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            Debug.WriteLine($"enviando mensaje:{message}");
            int bytesSent = socket.Send(data);

            Debug.WriteLine($"bytes enviados:{bytesSent}");

        }
        else
        {
            Debug.WriteLine("socket no esta conectado");
        }
    }

    public void Disconnect()
    {
        HandleDisconnection();
    }
}
