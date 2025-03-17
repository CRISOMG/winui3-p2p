using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using p2p.Contexts;
using p2p.Controllers;
using p2p.Models;

namespace p2p.Controls
{
    public sealed partial class SocketMessagesControl : UserControl
    {
        public SocketMessagesViewModel ViewModel { get; private set; }
        private readonly ApplicationContext context;

        public SocketMessagesControl()
        {
            InitializeComponent();
            ViewModel = new SocketMessagesViewModel();
            DataContext = ViewModel;

            context = App.AppContext;
            context.SocketManager.MessageReceived += OnMessageReceived;
            MessageListView.ItemsSource = ViewModel.ReceivedMessages;

            this.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            context.SocketManager.MessageReceived -= OnMessageReceived;
            Debug.WriteLine("SocketMessagesControl descargado y eventos limpiados.");
        }

        private async void OnMessageReceived(string message)
        {
            Debug.WriteLine($"Mensaje recibido: {message}");

            DispatcherQueue.TryEnqueue(() =>
            {
                ViewModel.ReceivedMessages.Add(message.Trim());

                if (MessageScrollViewer.VerticalOffset >= (MessageScrollViewer.ScrollableHeight - 200))
                {
                    MessageScrollViewer.UpdateLayout();
                    MessageScrollViewer.ChangeView(null, MessageScrollViewer.ScrollableHeight, null);
                }
            });
        }

        private void SendToServer_Click(object sender, RoutedEventArgs e)
        {
            SendToServerOrClient();
        }

        private void SendToClient_Click(object sender, RoutedEventArgs e)
        {
            SendToServerOrClient(isSendingToClient: true);
        }
        private void ClearMessages_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearMessages();
        }
        
        private void SendToServerOrClient(bool isSendingToClient = false)
        {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                if (isSendingToClient && ViewModel?.SelectedClientAddress != null)
                {
                    // Enviar mensaje al cliente seleccionado
                    //context.SocketManager.SendMessage(message, selectedClientAddress);
                    context.SocketManager.SendMessage(message); 
                }
                else
                {
                    // Enviar mensaje al servidor
                    context.SocketManager.SendMessage(message);
                }

                //MessageTextBox.Text = ""; // Limpiar el TextBox
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendToServerOrClient();
                e.Handled = true;
                MessageTextBox.Text = "";
            }
        }
    }

    public partial class SocketMessagesViewModel : ObservableObject
    {
        private readonly ApplicationContext context;
        public ObservableCollection<string> ReceivedMessages { get; } = new();


        [ObservableProperty]
        public string? selectedClientAddress = null;
        public SocketMessagesViewModel()
        {
            context = App.AppContext;
        }

        public void StartDiscovery() => context.MdnsController.StartDiscovery();

        public void AdvertiseService() => context.MdnsController.AdvertiseService();

        public void ClearMessages() => ReceivedMessages.Clear();
    }
}
