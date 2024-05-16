using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YourNamespace.ViewModels
{
    public class TcpClient
    {
        private Socket _clientSocket;
        private CancellationTokenSource _cancellationTokenSource;
        private string ipAddress { get; set; }
        private int port { get; set; }

        public event Action<string> OnMessageReceived;

        public TcpClient(string ipAddress, int port)
        {
           this.ipAddress = ipAddress; this.port = port;
            _cancellationTokenSource = new CancellationTokenSource();
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string username)
        {
            _clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectCallback, null);
        }

        public void Disconnect()
        {
            _cancellationTokenSource.Cancel();

            if (_clientSocket.Connected)
            {
                SendMessage("/disconnect");
                _clientSocket.Close();
            }
        }

        public void SendMessage(string message)
        {
            if (_clientSocket.Connected)
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                _clientSocket.Send(data);
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            _clientSocket.EndConnect(result);

            SendMessage($"/username TODO");  //TODO username

            Task.Factory.StartNew(() =>
            {
                ReceiveMessages();
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void ReceiveMessages()
        {
            byte[] receiveBuffer = new byte[1024];
            int bytesReceived;

            while ((bytesReceived = _clientSocket.Receive(receiveBuffer)) > 0)
            {
                string message = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, bytesReceived);

                OnMessageReceived?.Invoke(message);
            }

            _clientSocket.Close();
        }
    }
}