﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClientChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CommunicationWithSserver communicationWithServer;

        public string IPAddressHost = "127.0.0.1";
        public int PortHost = 5555;
        public string UserName = "User";
        private ObservableCollection<string> listUsers;
        private ObservableCollection<Message> listMessage;
        public MainWindow()
        {
            InitializeComponent();

            listUsers = new ObservableCollection<string>();
            listBoxListUsers.ItemsSource = listUsers;

            listMessage = new ObservableCollection<Message>();
            listBoxistReciveMessage.ItemsSource = listMessage;
        }

        private void ConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Нажали подключиться");
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
        }
            

        public void ChangeStatusConnection(bool IsConnect)
        {
            // Получить диспетчер от текущего окна и использовать его для вызова кода обновления
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    if (IsConnect)
                        ellipseStatusConnection.Fill = new SolidColorBrush(Colors.Green);
                    else
                        ellipseStatusConnection.Fill = new SolidColorBrush(Colors.Red);
                }
            );
        }
     
        // отправка сообщений
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if(!textBoxSendMessage.Text.Trim().Equals(""))
                if(communicationWithServer != null)
                    communicationWithServer.SendMessageToServer(textBoxSendMessage.Text);
        }
      
        //Вывод сообщения на экран
        public void SendToListReciveMessage(string message, bool iSentIt)
        {
                // Получить диспетчер от текущего окна и использовать его для вызова кода обновления
              this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,(ThreadStart)delegate ()
                {
                    Message messageObject = new Message();
                    messageObject.Text = message;

                    if (iSentIt) messageObject.Side = "Right";
                    else
                    {
                        messageObject.Side = "Left";

                        if (message.StartsWith("++ "))
                            listUsers.Add( message.Substring(3,message.IndexOf(":") - 3));
                        else if (message.StartsWith("-- "))
                            listUsers.Remove(message.Substring(3, message.IndexOf(":") - 3));
                    }

                    listMessage.Add(messageObject);
                }
            );
           
        }
      
        private void ClosingProgram(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (communicationWithServer != null)
            {
                communicationWithServer.SendMessageToServer("disconnect");
                communicationWithServer.Disconnect();
            }         
        }

        private void OpenSettingConnectionWindow_Click(object sender, RoutedEventArgs e)
        {
            SettingConnectionWindow settingConnectionWindow = new SettingConnectionWindow(IPAddressHost, PortHost, UserName);
            settingConnectionWindow.Owner = this;
            settingConnectionWindow.Show();
        }

        public class Message
        {
            public string Name { get; set; } 
            public string Text { get; set; } 
            public string Side { get; set; } 
        }
    }
}
