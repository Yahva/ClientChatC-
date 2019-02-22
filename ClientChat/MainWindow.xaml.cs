using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClientChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*public string IPAddressHost = "127.0.0.1";
        public int PortHost = 5555;
        public string UserName = "User";*/

        //private ObservableCollection<string> listUsers;
        //private ObservableCollection<Message> listMessage;

        public ApplicationViewModel applicationViewModel;
        public MainWindow()
        {
            InitializeComponent();

            applicationViewModel = new ApplicationViewModel(this);
            DataContext = applicationViewModel;
        }

                 
        private void ClosingProgram(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (applicationViewModel.communicationWithServer != null)
            {
                applicationViewModel.communicationWithServer.SendMessageToServer("disconnect");
                applicationViewModel.communicationWithServer.Disconnect();
            }         
        }

        private void OpenSettingConnectionWindow_Click(object sender, RoutedEventArgs e)
        {
            SettingConnectionWindow settingConnectionWindow = new SettingConnectionWindow(applicationViewModel.IPAddressHost, applicationViewModel.PortHost, applicationViewModel.UserName);
            settingConnectionWindow.Owner = this;
            settingConnectionWindow.Show();
        }
        
    }

    public class Message
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Side { get; set; }
    }
}
