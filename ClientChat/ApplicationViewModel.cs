using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClientChat
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public string IPAddressHost = "127.0.0.1";
        public int PortHost = 5555;
        public string UserName = "User";

        private MainWindow _mainWindow;

        internal CommunicationWithSserver communicationWithServer;
        private string sendMessage;

        public ObservableCollection<string> listUsers { get; set; }
        public ObservableCollection<Message> listMessage { get; set; }

        public ApplicationViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            listUsers = new ObservableCollection<string>();
            listMessage = new ObservableCollection<Message>();
            sendMessage = "";
        }


        public string SendMessage
        {
            get { return sendMessage; }
            set
            {
                sendMessage = value;
                OnPropertyChanged("SendMessage");
            }
        }

        // команда отправки сообщения
        private RelayCommand sendCommand;
        public RelayCommand SendMessageCommand
        {
            get
            {
                return sendCommand ??
                  (sendCommand = new RelayCommand(obj =>
                      {
                          if (communicationWithServer != null)
                          {
                              communicationWithServer.SendMessageToServer(SendMessage);

                              SendToListReciveMessage(SendMessage, true);
                              SendMessage = "";
                          }
                      },(obj) => !SendMessage.Trim().Equals(""))
                  );
            }
        }

        //Вывод сообщения на экран
        public void SendToListReciveMessage(string message, bool iSentIt)
        {
            // Получить диспетчер от текущего окна и использовать его для вызова кода обновления
            _mainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                Message messageObject = new Message();
                messageObject.Text = message;

                if (iSentIt) messageObject.Side = "Right";
                else
                {
                    messageObject.Side = "Left";

                    if (message.StartsWith("++ "))
                        listUsers.Add(message.Substring(3, message.IndexOf(":") - 3));
                    else if (message.StartsWith("-- "))
                        listUsers.Remove(message.Substring(3, message.IndexOf(":") - 3));
                }

                listMessage.Add(messageObject);
            }
          );

        }

        private RelayCommand connectToServerCommand;
        public RelayCommand ConnectToServerCommand
        {
            get
            {
                return connectToServerCommand ??
                  (connectToServerCommand = new RelayCommand(obj =>
                  {
                      IPEndPoint ipPointHost = new IPEndPoint(IPAddress.Parse(IPAddressHost), PortHost);
                      // Создаём конечную точку сервера
                      if (communicationWithServer == null)
                      {
                          communicationWithServer = new CommunicationWithSserver(UserName, ipPointHost, this);
                      }
                      else if (communicationWithServer.client != null)
                      {
                          //if (!communicationWithServer.client.Connected)
                          {
                              communicationWithServer.RunClient(ipPointHost);
                          }

                      }
                  }));
            }
        }

       
        public void ChangeStatusConnection(bool IsConnect)
        {
            // Получить диспетчер от текущего окна и использовать его для вызова кода обновления
            _mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (IsConnect)
                    _mainWindow.ellipseStatusConnection.Fill = new SolidColorBrush(Colors.Green);
                else
                    _mainWindow.ellipseStatusConnection.Fill = new SolidColorBrush(Colors.Red);
            }
            );
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
