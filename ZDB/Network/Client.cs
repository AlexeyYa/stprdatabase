using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZDB.Network
{
    class Client
    {
        TcpClient tcpClient;
        string server = "127.0.0.1";
        int port = 8888;
        NetworkStream stream = null;

        public Client(string serverIP, int serverPort)
        {
            server = serverIP;
            port = serverPort;

            Thread clientThread = new Thread(new ThreadStart(Run));
            clientThread.Start();
        }

        public void Run()
        {
            try
            {
                tcpClient = new TcpClient(server, port);

                stream = tcpClient.GetStream();
                byte[] data = new byte[256];
                while (true)
                {
                    try
                    {
                        GetMessage();
                    }
                    catch
                    {

                    }
                }
            }
            finally
            {
                Close();
            }
        }

        public object GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder stringBuilder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return stringBuilder.ToString();
        }

        public void Close()
        {
            if (stream != null)
                stream.Close();
            if (tcpClient != null)
                tcpClient.Close();

        }
    }
}
