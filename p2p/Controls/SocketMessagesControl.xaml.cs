using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using p2p.Contexts;
using p2p.Controllers;
using p2p.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace p2p.Controls
{
    public sealed partial class SocketMessagesControl : UserControl
    {
        public SocketMessagesViewModel ViewModel { get; private set; }

        public SocketMessagesControl()
        {
            this.InitializeComponent();
            ViewModel = new SocketMessagesViewModel(DispatcherQueue);
            this.DataContext = ViewModel;
            MessageListView.ItemsSource = ReceivedMessages;
            this.Closed += OnWindowClosed;

        }

        private void OnWindowClosed(object sender, WindowEventArgs e)
        {
            context.SocketManager.MessageReceived -= OnMessageReceived;
            Debug.WriteLine("MainWindow cerrada y eventos limpiados.");
        }
    }

    public partial class SocketMessagesViewModel : ObservableObject {
        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        public SocketMessagesViewModel(Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue)
        {
            context = App.AppContext;
            context.SocketManager.MessageReceived += OnMessageReceived;
            deviceController = new DeviceController(deviceList);

        }
        private DeviceListModel deviceList = new DeviceListModel();
        private DeviceController deviceController;
        private readonly ApplicationContext context;
        public ObservableCollection<string> ReceivedMessages { get; } = new ObservableCollection<string>();
        public ObservableCollection<DiscoveredService> discoveredServicesList { get; } = new ObservableCollection<DiscoveredService>();

     

        private async void OnMessageReceived(string message)
        {
            Debug.WriteLine($"Mensaje recibido: {message.ToString()}");

            _dispatcherQueue.TryEnqueue(() =>
            {
                ReceivedMessages.Add(message.Replace("\n", "").Trim());

                double verticalOffset = MessageScrollViewer.VerticalOffset;
                double scrollableHeight = MessageScrollViewer.ScrollableHeight;
                bool isAtBottom = verticalOffset >= (scrollableHeight - 200);
                if (isAtBottom)
                {
                    MessageScrollViewer.UpdateLayout();
                    MessageScrollViewer.ChangeView(null, MessageScrollViewer.ScrollableHeight, null);
                }
            });

        }

        private void ScanDevices(object sender, RoutedEventArgs e)
        {
            deviceController.ScanWiFiDirectDevices();
            // Asigna el origen de elementos del ListView directamente

        }
    
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                context.SocketManager.SendMessage(message);
                MessageTextBox.Text = "";
            }
        }
        private void MessageTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string message = MessageTextBox.Text;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    context.SocketManager.SendMessage(message);
                    MessageTextBox.Text = "";
                }
                e.Handled = true;
            }
        }
        private void SendToServer_Click(object sender, RoutedEventArgs e)
        {
            SendToServerOrClient();
        }

        private void SendToClient_Click(object sender, RoutedEventArgs e)
        {
            SendToServerOrClient(isSendingToClient: true);
        }

        private void SendToServerOrClient(bool isSendingToClient = false)
        {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                //if (isSendingToClient && selectedClientAddress != null)
                //{
                //    // Enviar mensaje al cliente seleccionado
                //    context.SocketManager.SendMessage(message, selectedClientAddress);
                //}
                //else
                //{
                //    // Enviar mensaje al servidor
                //    context.SocketManager.SendMessage(message);
                //}
                context.SocketManager.SendMessage(message);

                //MessageTextBox.Text = ""; // Limpiar el TextBox después de enviar
            }
        }
        private void ClearMessages_Click(object sender, RoutedEventArgs e)
        {
            ReceivedMessages.Clear(); // Limpiar la lista de mensajes
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
