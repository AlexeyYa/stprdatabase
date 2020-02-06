using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZDB.Database;

namespace ZDB.Network
{
    class Server
    {
        static TcpListener tcpListener;
        List<ClientObject> clients = new List<ClientObject>();

        public ObservableCollection<Entry> destDb { get; set; }

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();

                while (true)
                {
                    // Setup client processing in new thread
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this, destDb);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch
            {
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(CollectionMessage message, string id)
        {
            IFormatter formatter = new BinaryFormatter();
            foreach (var client in clients)
            {
                if (client.Id != id)
                {
                    formatter.Serialize(client.stream, message);
                }
            }
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop();

            foreach (var client in clients)
            {
                client.Close();
            }
        }
    }

    class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream stream { get; private set; }
        IFormatter formatter = new BinaryFormatter();

        public TcpClient client;
        Server server;

        ObservableCollection<Entry> db;

        public ClientObject(TcpClient tcpClient, Server serv, ObservableCollection<Entry> database)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serv;
            db = database;

            server.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                stream = client.GetStream();
                while (true)
                {
                    try
                    {
                        var message = GetMessage();

                        switch (message.action)
                        {
                            case "reqDB":
                                SendDB();
                                break;
                            case "add":
                                if (message.entry is Entry newEntry)
                                {
                                    db.Add(newEntry);
                                    SendResponce("S_OK");
                                }
                                server.BroadcastMessage(message, Id);
                                break;
                            case "cha":
                                if (message.entry is Entry chaEntry)
                                {
                                    Entry destination = db.First(r => r.Number == chaEntry.Number);
                                    if (destination != null && 
                                        destination[message.propertyName].ToString() == message.oldValue)
                                    {
                                        destination[message.propertyName] = chaEntry[message.propertyName];
                                        SendResponce("S_OK");
                                    }
                                }
                                server.BroadcastMessage(message, Id);
                                break;
                            case "rem":
                                if (message.entry is Entry oldEntry)
                                {
                                    var removedEntry = db.First(r => r.Number == oldEntry.Number);
                                    if (db.Remove(removedEntry))
                                        SendResponce("S_OK");
                                }

                                server.BroadcastMessage(message, Id);
                                break;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            finally
            {
                Close();
            }
        }

        private void SendResponce(string status)
        {
            CollectionMessage change = new CollectionMessage
            {
                action = "status",
                entry = status,
                newValue = "",
                oldValue = "",
                propertyName = ""
            };
            formatter.Serialize(stream, change);
        }

        private CollectionMessage GetMessage()
        {
            CollectionMessage message = (CollectionMessage)formatter.Deserialize(stream);
            return message;
        }

        private void SendDB()
        {
            List<Entry> list = new List<Entry>(db);
            CollectionMessage change = new CollectionMessage
            {
                action = "reqDB",
                entry = list,
                newValue = "",
                oldValue = "",
                propertyName = ""
            };
            formatter.Serialize(stream, change);
        }

        protected internal void Close()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
