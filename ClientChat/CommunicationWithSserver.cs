using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ClientChat
{

    internal class CommunicationWithSserver
    {
        private string _userName = "User"; // Имя пользователя
        public TcpClient client;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private MainWindow _mainWindow;

        public CommunicationWithSserver(string userName, IPEndPoint ipPointHost, MainWindow mainWindow)
        {
            _userName = userName;
            _mainWindow = mainWindow;
            RunClient(ipPointHost);
        }

        public void RunClient(IPEndPoint ipPointHost)
        {
            client = new TcpClient();
            try
            {
                client.Connect(ipPointHost); // Подключение клиента к серверу
                _stream = client.GetStream(); // Получаем поток для отправки и получения данных

                SendMessageToServer(_userName);//Отправляем имя пользователя на сервер

                // Запускаем новый поток для получения данных
                _receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                _receiveThread.Start(); //старт потока

                _mainWindow.SendToListReciveMessage("Подключение произведено успешно!", true);
                _mainWindow.ChangeStatusConnection(true);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // получение сообщений с сервера
        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        Console.WriteLine("Ждём сообщение от сервера");
                        bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (_stream.DataAvailable);

                    string message = builder.ToString();

                    if (message.Equals("disconnect")) throw new Exception("Серевер рассоединился");
                    //вывод сообщения на интерфес                 
                    if (_mainWindow != null)
                    {
                        _mainWindow.SendToListReciveMessage(message, false);
                    } 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Disconnect();
                    break;
                }
            }
        }
        //отправка сообщений на сервер 
        public void SendMessageToServer(string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                if (_mainWindow != null)
                {
                    _mainWindow.SendToListReciveMessage(message, true);
                    _mainWindow.textBoxSendMessage.Clear();
                }
                   
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_mainWindow != null)
            {
                _mainWindow.SendToListReciveMessage("Подключение прервано!", true); //соединение было прервано
                _mainWindow.ChangeStatusConnection(false);
            }

            if (_stream != null)
                _stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            if (_receiveThread != null && _receiveThread.IsAlive) _receiveThread.Abort();
            
        }

    }




}
