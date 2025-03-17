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
using p2p.ViewModels;

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
}
