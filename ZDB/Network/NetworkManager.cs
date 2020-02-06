using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZDB.Database;

namespace ZDB.Network
{
    class NetworkManager
    {
        private bool isServer = true;
        private ObservableCollection<Entry> dbCollection;


        // Server variables
        private Server server;
        private Thread listenThread;

        // Client variables
        private Client client;


        public NetworkManager(bool isServer, NetworkCollection localCollection, 
            ObservableCollection<Entry> dbCollection = null)
        {
            this.isServer = isServer;

            if (isServer)
            {
                try
                {
                    server = new Server();
                    server.destDb = dbCollection;
                    listenThread = new Thread(new ThreadStart(server.Listen));
                    listenThread.Start();
                }
                catch
                {
                    server.Disconnect();
                }


                this.dbCollection = dbCollection;
            }
            
            client = new Client("127.0.0.1", 8888, localCollection);
            localCollection.client = client;
        }

        public void Close()
        {
            if (isServer) server.Disconnect();
            client.Close();
        }
    }
}
