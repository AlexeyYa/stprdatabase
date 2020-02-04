using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ZDB.Network
{
    class Server
    {
        IPAddress localAddr = IPAddress.Loopback;
        int port = 8888;
        static TcpListener tcpListener;
        List<Client> clients = new List<Client>();

        protected internal void AddConnection(Client clientObject)
        {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(string id)
        {
            
        }

        protected internal void Listen()
        {
            try
            {

            }
            catch
            {
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            foreach (var client in clients)
            {
                
            }
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop();

            foreach (var client in clients)
            {
                client.Close();
            }

            throw new NotImplementedException();
        }
    }


}
