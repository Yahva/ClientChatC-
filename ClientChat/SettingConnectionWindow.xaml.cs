using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace ClientChat
{
    /// <summary>
    /// Логика взаимодействия для SettingConnectionWindow.xaml
    /// </summary>
    public partial class SettingConnectionWindow : Window
    {
        public SettingConnectionWindow(string IPAddressHost, int PortHost, string UserName)
        {
            InitializeComponent();

            textBoxIPAddress.Text = IPAddressHost;
            textBoxPort.Text = PortHost.ToString();
            textBoxUserName.Text = UserName;
        }

        private void SaveConnectionInfo_Click(object sender, RoutedEventArgs e)
        {
            string patternIPAddress = @"\d{3}\.\d{3}\.\d{3}.\d{3}";
            string patternPort = @"\d{4}\d?$";

            if (Regex.IsMatch(textBoxIPAddress.Text, patternIPAddress))
            {
                ((MainWindow)this.Owner).IPAddressHost = textBoxIPAddress.Text;
            }
            else
            {
                textBoxIPAddress.Text = ((MainWindow)this.Owner).IPAddressHost;
            }

            if (Regex.IsMatch(textBoxPort.Text, patternPort))
            {
                ((MainWindow)this.Owner).PortHost = Convert.ToInt32(textBoxPort.Text);
            }
            else
            {
                textBoxPort.Text = ((MainWindow)this.Owner).PortHost.ToString();
            }

            if (!textBoxUserName.Text.Trim().ToString().Equals(""))
            {
                ((MainWindow)this.Owner).UserName = textBoxUserName.Text;
            }
            else
            {
                textBoxUserName.Text = ((MainWindow)this.Owner).UserName;
            }

           
        }
    }
}
