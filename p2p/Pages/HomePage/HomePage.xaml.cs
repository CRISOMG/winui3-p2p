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
using p2p.Contexts;
using CommunityToolkit.Mvvm.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace p2p.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private readonly ApplicationContext context;
        public RelayCommand<string> NavigateTo { get; }

        public HomePage()
        {
            this.InitializeComponent();
            context = App.AppContext;


            NavigateTo = new RelayCommand<string>((pageType) =>
            {
                //var page = typeof(SocketMessagesPage);
                //var page = Type.GetType(pageType);
                //var page = Type.GetType(pageType) ?? Type.GetType($"p2p.Pages.{pageType}, {typeof(SocketMessagesViewModel).Assembly.FullName}");

                var page = Type.GetType($"p2p.Pages.{pageType}, {typeof(App).Assembly.FullName}");
                if (page != null)
                {
                    MainWindow.NavigationService.Navigate(page);
                }
            });

            this.DataContext = this;
        }

     
    }
}
