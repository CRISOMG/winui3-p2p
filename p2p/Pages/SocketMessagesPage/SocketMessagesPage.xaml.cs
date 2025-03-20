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
using CommunityToolkit.Mvvm.Input;
using p2p.Controllers;
using System.Diagnostics;

namespace p2p.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class SocketMessagesViewModel
    {
        public RelayCommand<string> NavigateTo { get; }
        public SocketMessagesViewModel()
        {

            NavigateTo = new RelayCommand<string>((pageType) =>
            {
                var page = Type.GetType(pageType) ?? Type.GetType($"p2p.Pages.{pageType}, {typeof(App).Assembly.FullName}");
                if (page != null)
                {
                    MainWindow.NavigationService.Navigate(page);
                }
            });
        }
    }
    public sealed partial class SocketMessagesPage : Page
    {

        public SocketMessagesViewModel ViewModel { get; }
        public SocketMessagesPage()
        {
            this.InitializeComponent();
            //MainWindow.MainFrame.Navigate();
            ViewModel = new SocketMessagesViewModel();
            this.DataContext = ViewModel;
          
        }
    }
}
