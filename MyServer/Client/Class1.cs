using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Client
    {

        private Socket _clientSocket;

        private Thread _thread;

        private byte[] _data = new byte[1024];

        public bool Connected
        {
            get { return _clientSocket != null&&_clientSocket.Connected; }
        }

        public Client(Socket clientSocket)
        {
            _clientSocket = clientSocket;

            _thread = new Thread(ReceiveMessage);

            _thread.Start();
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                ///判断连接是否断开
                if (_clientSocket.Poll(10,SelectMode.SelectRead))
                {
                    _clientSocket.Close();
                    break;
                }

                int length = _clientSocket.Receive(_data);

                if (length > 0)
                {
                    string message = Encoding.UTF8.GetString(_data, 0, length);

                    Console.WriteLine("收到" + message);

                    Program.BroadcastMessage(message);
                }
            }
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            _clientSocket.Send(data);
        }
    }

    internal class Program
    {
        static List<Client> clientList = new List<Client>();

        public static void BroadcastMessage(string message)
        {
            var notConnectedList = new List<Client>();

            foreach (Client client in clientList)
            {
                if (client.Connected)
                {
                    client.SendMessage(message);
                }
                else
                {
                    notConnectedList.Add(client);
                }
            }


            while (notConnectedList.Count>0)
            {
                Console.WriteLine("一个服务器断开了");
                notConnectedList.RemoveAt(0);
            }

        }

        static void Main(string[] args)
        {
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            tcpServer.Bind(new IPEndPoint(IPAddress.Parse("192.168.65.11"), 7788));
            tcpServer.Listen(100);

            Console.WriteLine("服务器启动成功");

            while (true)
            {
                Socket clientSocket = tcpServer.Accept();
                Console.WriteLine("有一个客户端过来了");
                Client client = new Client(clientSocket);
                clientList.Add(client);
            }

        }

   


    }




}
