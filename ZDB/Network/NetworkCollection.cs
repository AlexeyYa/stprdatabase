using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
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
    [Serializable]
    class CollectionMessage
    {
        public object entry { get; set; }
        public string oldValue { get; set; }
        public string newValue { get; set; }
        public string action { get; set; }
        public string propertyName { get; set; }

    }

    class NetworkCollection : ObservableCollection<Entry>
    {
        public Client client { get; set; }
        public NetworkCollection()
        {
            // Load db
            this.CollectionChanged += ItemsCollectionChanged;

        }

        public Entry FindByID(int id)
        {
            return Items.First(e => e.Number == id);
        }

        private int IndexOfItem(int id)
        {
            return Items.IndexOf(FindByID(id));
        }

        protected override void RemoveItem(int index)
        {
            UnsubscribeItem(Items[index]);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, Entry item)
        {
            UnsubscribeItem(Items[index]);
            SubscribeItem(item);
            base.SetItem(index, item);
        }

        public void AddRange(IEnumerable<Entry> collection)
        {
            this.CollectionChanged -= ItemsCollectionChanged;

            CheckReentrancy();

            foreach (var entry in collection)
            {
                entry.PropertyChangedEx += ItemChanged;
            }

            int index = Count;
            var target = (List<Entry>)Items;
            target.InsertRange(index, collection);

            IList<Entry> list = new List<Entry>(collection);
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(
            //        NotifyCollectionChangedAction.Add, list, index));
            
            this.CollectionChanged += ItemsCollectionChanged;
        }

        protected override void InsertItem(int index, Entry item)
        {
            SubscribeItem(item);
            base.InsertItem(index, item);
        }

        private void SubscribeItem(Entry entry)
        {
            entry.PropertyChangedEx += ItemChanged;
        }

        private void UnsubscribeItem(Entry entry)
        {
            entry.PropertyChangedEx -= ItemChanged;
        }

        // ADD MUTEX !!!!!!!!!!!!!!!!!
        // Server - processing sent data and user input
        // Client - sending data to server
        private void ItemChanged(object sender, PropertyChangedExtendedEventArgs e)
        {
            if (sender is Entry entry)
            {
                client.ItemChanged(entry, e);

                /*if (isServer)
                {
                    Entry destination = dbCollection.First(r => r.Number == entry.Number);
                    if (destination[e.PropertyName].ToString() == e.OldValue)
                    {
                        destination[e.PropertyName] = entry[e.PropertyName];
                    }
                    else
                    {
                        throw new MemberAccessException("Error: different initial values");
                    }

                    Stream stream = new FileStream("teststream", FileMode.Create, FileAccess.Write, FileShare.None);
                    IFormatter formatter = new BinaryFormatter();
                    DBChangeObject change = new DBChangeObject { action = "cha", 
                                                                 entry = entry,
                                                                 newValue = e.NewValue,
                                                                 oldValue = e.OldValue,
                                                                 propertyName = e.PropertyName};
                    formatter.Serialize(stream, change);
                    stream.Close();

                    

                    //stream = new FileStream("teststream", FileMode.Open, FileAccess.Read, FileShare.Read);
                    //DBChangeObject testE = (DBChangeObject)formatter.Deserialize(stream);
                    //testE.Number = this.Last().Number + 1;
                    //this.Add(testE);
                    //stream.Close();
                }
                else
                {*/
                    // serialize sender and e then send
                    

                    //throw new NotImplementedException();
                //}
            }
            else
            {
                throw new NotImplementedException("Sender is not Entry!");
            }
        }

        private void ItemChangedRecieved(object sender, PropertyChangedExtendedEventArgs e)
        {
            //if (sender is Entry entry)
            //{
            //    if (isServer)
            //    {

            //        Entry destination = this.First(r => r.Number == entry.Number);
            //        if (destination[e.PropertyName].ToString() == e.OldValue)
            //        {
            //            destination[e.PropertyName] = entry[e.PropertyName];
            //        }
            //        else
            //        {
            //            throw new MemberAccessException("Error: different initial values");
            //        }
            //    }
            //    else
            //    {
            //        // serialize sender and e then send
            //        throw new NotImplementedException();
            //    }
            //}
        }

        // ADD MUTEX
        // Server - processing sent data and user input, send responce with id number
        // Client - sending data to server
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (isServer)
            //{
            //    if (e.NewItems != null)
            //    {
            //        foreach (Entry entry in e.NewItems)
            //        {
            //            if (dbCollection.Last().Number >= entry.Number)
            //            {
            //                entry.Number = dbCollection.Last().Number + 1;
            //            }
            //            dbCollection.Add(entry);
            //        }
            //    }
            //    if (e.OldItems != null)
            //    {
            //        foreach (Entry entry in e.OldItems)
            //        {
            //            dbCollection.Remove(dbCollection.First(r => r.Number == entry.Number));
            //        }
            //    }

            //} else
            //{
            //    if (e.NewItems != null)
            //    {
            //        foreach (Entry entry in e.NewItems)
            //        {

            //            Stream stream = new FileStream("teststream", FileMode.Create, FileAccess.Write, FileShare.None);
            //            IFormatter formatter = new BinaryFormatter();
            //            CollectionMessage change = new CollectionMessage
            //            {
            //                action = "add",
            //                entry = entry,
            //                newValue = "",
            //                oldValue = "",
            //                propertyName = ""
            //            };
            //            formatter.Serialize(stream, change);
            //            stream.Close();
            //        }
            //    }
            //    if (e.OldItems != null)
            //    {
            //        foreach (Entry entry in e.OldItems)
            //        {

            //            Stream stream = new FileStream("teststream", FileMode.Create, FileAccess.Write, FileShare.None);
            //            IFormatter formatter = new BinaryFormatter();
            //            CollectionMessage change = new CollectionMessage
            //            {
            //                action = "rem", // Who is Rem?
            //                entry = entry,
            //                newValue = "",
            //                oldValue = "",
            //                propertyName = ""
            //            };
            //            formatter.Serialize(stream, change);
            //            stream.Close();
            //        }
            //    }
            //    // Get response
            //    //throw new NotImplementedException();
            //}
            client.CollectionChanged(e);
        }

        private void ItemsCollectionChangedRecieved(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (isServer)
            //{
            //    if (e.NewItems != null)
            //    {
            //        foreach (Entry entry in e.NewItems)
            //        {
            //            if (this.Last().Number >= entry.Number)
            //            {
            //                entry.Number = this.Last().Number + 1;
            //            }
            //            this.Add(entry);
            //        }
            //    }
            //    if (e.OldItems != null)
            //    {
            //        foreach (Entry entry in e.OldItems)
            //        {
            //            this.Remove(this.First(r => r.Number == entry.Number));
            //        }
            //    }
            //}
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }
    }
}
