using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace icqwpf
{
    public class TcpServer
    {
        private int port { get; set; }
        private Socket _serverSocket;
        private CancellationTokenSource _cancellationTokenSource;
        private ConcurrentDictionary<string, Socket> _connectedClients;

        public event Action<long, string> OnMessageReceived; // long -> USER model
        public event Action<long> OnUserConnected;
        public event Action<long> OnUserDisconnected;

        public TcpServer(int port)
        {
            this.port = port;
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _cancellationTokenSource = new CancellationTokenSource();
            _connectedClients = new ConcurrentDictionary<string, Socket>();
        }

        public void Start()
        {
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _serverSocket.Listen(10);

            Task.Factory.StartNew(() =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Socket clientSocket = _serverSocket.Accept();

                    Task.Factory.StartNew(() =>
                    {
                        HandleClient(clientSocket);
                    }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _serverSocket.Close();
        }

        private void HandleClient(Socket clientSocket)
        {
            byte[] receiveBuffer = new byte[1024];
            int bytesReceived;

            while ((bytesReceived = clientSocket.Receive(receiveBuffer)) > 0)
            {
                string message = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, bytesReceived);

                if (message.StartsWith("/username "))
                {
                    string username = message.Substring(9);
                    _connectedClients[username] = clientSocket;
/*
                    Long user = new User
                    {
                        Username = username
                    };*/

 /*                   OnUserConnected?.Invoke(user);*/
                }
                else if (message == "/disconnect")
                {
                    string username = GetUsernameByClient(clientSocket);

                    if (!string.IsNullOrEmpty(username))
                    {
                        _connectedClients.TryRemove(username, out Socket removedClient);
/*
                        User user = new User
                        {
                            Username = username
                        };
*//*
                        OnUserDisconnected?.Invoke(user);*/
                    }

                    break;
                }
                else
                {
                    string username = GetUsernameByClient(clientSocket);
/*
                    if (!string.IsNullOrEmpty(username))
                    {
                        OnMessageReceived?.Invoke(new User { Username = username }, message);
                    }*/
                }
            }

            clientSocket.Close();
        }

        private string GetUsernameByClient(Socket clientSocket)
        {
            foreach (var item in _connectedClients)
            {
                if (item.Value.Equals(clientSocket))
                {
                    return item.Key;
                }
            }

            return string.Empty;
        }
    }
}