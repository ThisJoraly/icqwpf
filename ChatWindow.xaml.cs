using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace icqwpf
{
    /// <summary>
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly Thread _receiveThread;
        private string _username;

        public ChatWindow()
        {
            InitializeComponent();

            _client = new TcpClient();
            _client.Connect("127.0.0.1", 12345);
            _stream = _client.GetStream();

            _receiveThread = new Thread(ReceiveMessages);
            _receiveThread.Start();

            SendMessage("/login " + _username);
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                var message = ReceiveMessage();
                if (message == null)
                {
                    break;
                }

                Dispatcher.Invoke(() =>
                {
                    MessagesListBox.Items.Add(message);
                });
            }
        }

        private string ReceiveMessage()
        {
            var buffer = new byte[1024];
            var bytesReceived = _stream.Read(buffer, 0, buffer.Length);
            if (bytesReceived == 0)
            {
                return null;
            }

            return Encoding.UTF8.GetString(buffer, 0, bytesReceived);
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageTextBox.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            SendMessage(message);

            MessageTextBox.Clear();
        }

        private void SendMessage(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            _stream.Write(buffer, 0, buffer.Length);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveThread.Abort();
            _stream.Close();
            _client.Close();

            Close();
        }
    }

}
