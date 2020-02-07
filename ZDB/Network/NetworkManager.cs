using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZDB.Database;

namespace ZDB.Network
{
    class NetworkManager
    {

        private bool isServer = false;
        
        // Server variables
        private Server server;
        private Thread listenThread;

        // Client variables
        private Client client;


        public NetworkManager()
        {
        }

        public void StartServer()
        {
            try
            {
                server = new Server();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
                isServer = true;
            }
            catch
            {

                server.Disconnect();
            }
        }

        public void StartClient(NetworkCollection localCollection, string ipDestination)
        {
            client = new Client(ipDestination, 8888, localCollection);
            localCollection.client = client;
        }

        public void Close()
        {
            if (server != null) server.Disconnect();
            if (client != null) client.Close();
        }
    }
}
