using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Main(Object Sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(new MainViewModel(new ConnectionHandler()));
            mainWindow.Title = "Message Sender";
            mainWindow.Show();
        }
    }
}
