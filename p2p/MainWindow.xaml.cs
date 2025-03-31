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

using Microsoft.UI.Xaml.Media.Animation;
using Windows.Devices.WiFiDirect;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using p2p.Pages;
using System.Collections.Generic;

using System.Diagnostics;
using System.IO;

using Windows.ApplicationModel;


namespace p2p
{
    public sealed partial class MainWindow : Window
    {
        private readonly ApplicationContext context;
        //public static Frame MainFrame { get; private set; }
        public static Frame InstanceFrame { get; private set; }
        public static NavigationController NavigationService { get; private set; } = new NavigationController();

        public class MyStringList
        {
            public string String1 { get; set; } = "p2p.Pages.SocketMessagesPage";
            public string String2 { get; set; } = "p2p.Pages.HomePage";
        }


public static void AddFirewallRule()
    {
            string exePath = Path.Combine(AppContext.BaseDirectory, "p2p.exe"); // Reemplaza "MyApp.exe" con el nombre real de tu ejecutable
            string script = $@"
$exePath = ""{exePath}""
New-NetFirewallRule -DisplayName ""Wi-Fi P2P App Inbound"" -Direction Inbound -Action Allow -Program $exePath -Profile Any
New-NetFirewallRule -DisplayName ""Wi-Fi P2P App Outbound"" -Direction Outbound -Action Allow -Program $exePath -Profile Any
exit
    ";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-ExecutionPolicy Bypass -NoProfile -Command \"{script}\"",
                Verb = "runas",
                UseShellExecute = false, // Permite capturar salida
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true // Asegura que no se abra una ventana de PowerShell
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                process.WaitForExit(); // Espera a que termine antes de continuar
            }
        }
    public MainWindow()
        {
            this.InitializeComponent();
            context = App.AppContext;
            InstanceFrame = this.MainFrame;
            //AddFirewallRule();
            NavigationService.Initialize(InstanceFrame);
            NavigationService.Navigate(typeof(HomePage));

            bool isPackaged = Package.Current != null;
            Debug.WriteLine($"La aplicación está empaquetada en MSIX: {isPackaged}");

            //ResourceDictionary resourceDictionary = new ResourceDictionary();
            //Application.Current.Resources.Add("PagesMap", NavigationService.PageList);
            //Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

            // Agregar el diccionario al ResourceDictionary
            //Application.Current.Resources.Add("MyStringList", new MyStringList());
            //Application.Current.Resources["PagesMap"] = NavigationController.PagesMap;

        }


    }

}
