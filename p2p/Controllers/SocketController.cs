using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class SocketManager
{
    //private readonly ApplicationContext context;
    public Socket socket;
    public event Action<string> MessageReceived;
    public event Action OnDisconnected;
 
    public event PropertyChangedEventHandler PropertyChanged;
    private bool _isConnected;
    public bool IsConnected
    {
        get => _isConnected;
        private set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                NotifyPropertyChanged(nameof(IsConnected)); // 🔥 Notificar cambios a la UI
            }
        }
    }
    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    //public SocketManager(ApplicationContext context)
    //{
    //    this.context = context;
    //}

    public async Task ConnectAsync(string ipAddress, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(ipAddress, port);
            Debug.WriteLine("Conexión establecida.");
            IsConnected = true;
            StartReceiving();
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
    private async void StartReceiving()
  {
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
        else { 
            Debug.WriteLine("socket no esta conectado");
        }
    }

    public void Disconnect()
    {
        HandleDisconnection();
    }
}
