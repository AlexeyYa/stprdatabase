using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZDB.Database;

namespace ZDB.Network
{
    class Client
    {
        string server;
        int port;
        TcpClient tcpClient;
        NetworkStream stream = null;
        NetworkCollection Entries { get; set; }
        IFormatter formatter;
        Thread recieveThread;
        ConcurrentQueue<CollectionMessage> sendQueue;

        public Client(string serverIP, int serverPort, NetworkCollection entries)
        {
            server = serverIP;
            port = serverPort;
            Entries = entries;
            formatter = new BinaryFormatter();
            sendQueue = new ConcurrentQueue<CollectionMessage>();

            Thread clientThread = new Thread(new ThreadStart(Run));
            clientThread.Start();
        }

        public void Run()
        {
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(server, port);
                stream = tcpClient.GetStream();

                recieveThread = new Thread(new ThreadStart(RecieveMessage));
                recieveThread.Start();
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        public void RequestDB()
        {
            var message = new CollectionMessage { action = "reqDB",
                                                  entry = "",
                                                  newValue = "",
                                                  oldValue = "",
                                                  propertyName = ""
                                                };

            formatter.Serialize(stream, message);
        }

        public void SendMessage(CollectionMessage message)
        {

        }

        public void RecieveMessage()
        {
            RequestDB();
            while (true)
            {
                try
                {
                    CollectionMessage message = (CollectionMessage)formatter.Deserialize(stream);

                    switch (message.action)
                    {
                        case "reqDB":
                            if (message.entry is List<Entry> collection)
                            {
                                App.Current.Dispatcher.Invoke((Action) delegate
                                {
                                    Entries.AddRange(collection);
                                });
                            } else
                            {
                                throw new InvalidOperationException("Couldn't load");
                            }
                            break;
                        case "add":
                            if (message.entry is Entry newEntry)
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    Entries.RecievedAdd(newEntry);
                                });
                            }
                            else
                            {
                                throw new InvalidOperationException("Couldn't add entries");
                            }
                            break;
                        case "cha":
                            if (message.entry is Entry chaEntry)
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    Entry destination = Entries.First(r => r.Number == chaEntry.Number);
                                    if (destination != null &&
                                        destination[message.propertyName].ToString() == message.oldValue)
                                    {
                                        destination[message.propertyName] = chaEntry[message.propertyName];
                                    }
                                });
                            }
                            else
                            {
                                throw new InvalidOperationException("Couldn't change entries");
                            }
                            break;
                        case "rem":
                            if (message.entry is Entry oldEntry)
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    Entries.RecievedRemove(oldEntry);
                                });
                            }
                            else
                            {
                                throw new InvalidOperationException("Couldn't remove entries");
                            }
                            break;
                        case "status":
                            if ((string)message.entry == "S_OK")
                                sendQueue.TryDequeue(out CollectionMessage successfulMessage);
                            else if ((string)message.entry == "S_ADD")
                            {
                                sendQueue.TryDequeue(out CollectionMessage successfulMessage);
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    ((Entry)successfulMessage.entry).Number = Int32.Parse(message.newValue);
                                });
                            }
                            else
                            {
                                sendQueue.TryDequeue(out CollectionMessage failedMessage);
                                sendQueue.Enqueue(failedMessage);
                                throw new IndexOutOfRangeException();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    int i = 0;
                }
            }
        }

        public void Close()
        {
            if (recieveThread != null)
            {
                recieveThread.Abort();
            }
            if (stream != null)
                stream.Close();
            if (tcpClient != null)
                tcpClient.Close();
        }

        public void ItemChanged(Entry entry, PropertyChangedExtendedEventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();
            CollectionMessage change = new CollectionMessage
            {
                action = "cha",
                entry = entry,
                newValue = e.NewValue,
                oldValue = e.OldValue,
                propertyName = e.PropertyName
            };
            sendQueue.Enqueue(change);
            formatter.Serialize(stream, change);
        }

        public void CollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Entry entry in e.NewItems)
                {
                    IFormatter formatter = new BinaryFormatter();
                    CollectionMessage change = new CollectionMessage
                    {
                        action = "add",
                        entry = entry,
                        newValue = "",
                        oldValue = "",
                        propertyName = ""
                    };
                    sendQueue.Enqueue(change);
                    formatter.Serialize(stream, change);
                }
            }
            if (e.OldItems != null)
            {
                foreach (Entry entry in e.OldItems)
                {
                    IFormatter formatter = new BinaryFormatter();
                    CollectionMessage change = new CollectionMessage
                    {
                        action = "rem", // Who is Rem?
                        entry = entry,
                        newValue = "",
                        oldValue = "",
                        propertyName = ""
                    };
                    sendQueue.Enqueue(change);
                    formatter.Serialize(stream, change);
                }
            }
        }
    }
}
