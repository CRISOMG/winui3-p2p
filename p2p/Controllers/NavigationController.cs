using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using p2p.Pages;

namespace p2p.Controllers
{


    public static class PageMap
    {
        public static string SocketMessagesPage => "SocketMessagesPage";
        public static string HomePage => "p2p.Pages.HomePage";
    }

    public class NavigationController
    {

        public static RelayCommand<string> NavigateTo = new((pageType) =>
        {
            var page = Type.GetType(pageType) ?? Type.GetType($"p2p.Pages.{pageType}, {typeof(App).Assembly.FullName}");
            if (page != null)
            {
                MainWindow.NavigationService.Navigate(page);
            }
        });
        private Frame _frame;

        public NavigationController()
        {

        }


        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate(Type pageType)
        {
            if (pageType != null)
            {
                _frame?.Navigate(pageType, null, new SuppressNavigationTransitionInfo());
            }
        }
    }
}
