using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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
        
        // Database
        private DatabaseContext db;
        static object locker = new object();

        public ObservableCollection<Entry> dbCollection { get; set; }

        public Server()
        {
            db = new DatabaseContext();
            db.Entries.Load();
            dbCollection = db.Entries.Local;
            /* IMPORT SAMPLE
             * var data = Import.LoadContents(@"E:\table_2019_utf.csv");
            foreach (Entry e in data)
            {
                dbCollection.Add(e);
            }
            db.SaveChanges();*/
        }

        // Database update
        protected internal void RequestDB(string Id)
        {
            lock (locker)
            {
                var client = clients.First(c => c.Id == Id);
                List<Entry> list = new List<Entry>(dbCollection);
                client.SendDB(list);
            }
        }
        protected internal int Add(Entry newEntry)
        {
            lock (locker)
            {
                    
                dbCollection.Add(newEntry);
                int result = dbCollection.Last().Number;
                db.SaveChanges();
                
                return result;
            }
        }
        protected internal bool Change(Entry chaEntry, string propertyName, string oldValue, string newValue)
        {
            lock (locker)
            {
                bool result = false;
                Entry destination = dbCollection.First(r => r.Number == chaEntry.Number);
                if (destination != null &&
                    destination[propertyName].ToString() == oldValue)
                {
                    destination[propertyName] = chaEntry[propertyName];
                    db.SaveChanges();
                    result = true;
                }
                else if (destination[propertyName].ToString() == newValue)
                {
                    result = true;
                }
                return result;
            }
        }
        protected internal bool Remove(Entry oldEntry)
        {
            lock (locker)
            {
                bool result = false;
                var removedEntry = dbCollection.FirstOrDefault(r => r.Number == oldEntry.Number);
                if (removedEntry != null)
                {
                    result = dbCollection.Remove(removedEntry);
                    db.SaveChanges();
                }
                return result;
            }
        }

        // Server part
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

                    ClientObject clientObject = new ClientObject(tcpClient, this, dbCollection);
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
            foreach (var client in clients)
            {
                client.Close();
            }

            tcpListener.Stop();
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
                                server.RequestDB(Id);
                                break;
                            case "add":
                                if (message.entry is Entry newEntry)
                                {
                                    int serverNumber = server.Add(newEntry);
                                    if (serverNumber != 0)
                                        SendResponce("S_ADD", newEntry.Number, serverNumber);
                                    else SendResponce("S_FUCKED");
                                }
                                server.BroadcastMessage(message, Id);
                                break;
                            case "cha":
                                if (message.entry is Entry chaEntry)
                                {
                                    if (server.Change(chaEntry, message.propertyName,
                                                        message.oldValue, message.newValue))
                                        SendResponce("S_OK");
                                    else SendResponce("S_FUCKED");
                                }
                                server.BroadcastMessage(message, Id);
                                break;
                            case "rem":
                                if (message.entry is Entry oldEntry)
                                {
                                    if (server.Remove(oldEntry))
                                        SendResponce("S_OK");
                                    else SendResponce("S_FUCKED");
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

        private void SendResponce(string status, int oldNumber, int newNumber)
        {
            CollectionMessage change = new CollectionMessage
            {
                action = "status",
                entry = status,
                newValue = oldNumber.ToString(),
                oldValue = newNumber.ToString(),
                propertyName = ""
            };
            formatter.Serialize(stream, change);
        }

        private CollectionMessage GetMessage()
        {
            CollectionMessage message = (CollectionMessage)formatter.Deserialize(stream);
            return message;
        }

        protected internal void SendDB(List<Entry> list)
        {
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
