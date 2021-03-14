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
        static object locker = new object();

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
            lock (locker)
            {
                UnsubscribeItem(Items[index]);
                base.RemoveItem(index);
            }
        }

        protected override void SetItem(int index, Entry item)
        {
            lock (locker)
            {
                UnsubscribeItem(Items[index]);
                SubscribeItem(item);
                base.SetItem(index, item);
            }
        }

        public void AddRange(IEnumerable<Entry> collection)
        {
            lock (locker)
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
        }

        protected override void InsertItem(int index, Entry item)
        {
            lock (locker)
            {
                SubscribeItem(item);
                base.InsertItem(index, item);
            }
        }

        private void SubscribeItem(Entry entry)
        {
            lock (locker)
            {
                entry.PropertyChangedEx += ItemChanged;
            }
        }

        private void UnsubscribeItem(Entry entry)
        {
            lock (locker)
            {
                entry.PropertyChangedEx -= ItemChanged;
            }
        }

        // ADD MUTEX !!!!!!!!!!!!!!!!!
        // Server - processing sent data and user input
        // Client - sending data to server
        private void ItemChanged(object sender, PropertyChangedExtendedEventArgs e)
        {
            if (sender is Entry entry)
            {
                client.ItemChanged(entry, e);
            }
            else
            {
                throw new NotImplementedException("Sender is not Entry!");
            }
        }

        // ADD MUTEX
        // Server - processing sent data and user input, send responce with id number
        // Client - sending data to server
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            client.CollectionChanged(e);
        }

        public void RecievedAdd(Entry entry)
        {
            CheckReentrancy();
            this.CollectionChanged -= ItemsCollectionChanged;

            this.Add(entry);
            this.CollectionChanged += ItemsCollectionChanged;
        }
        public void RecievedRemove(Entry entry)
        {
            CheckReentrancy();
            this.CollectionChanged -= ItemsCollectionChanged;

            var removedEntry = this.First(r => r.Number == entry.Number);
            this.Remove(removedEntry);
            this.CollectionChanged += ItemsCollectionChanged;
        }
        public void RecievedChange(Entry entry, string propertyName, string oldValue, string newValue)
        {

            Entry destination = this.First(r => r.Number == entry.Number);
            destination.PropertyChangedEx -= ItemChanged;
            if (destination != null &&
                destination[propertyName].ToString() == oldValue)
            {
                destination[propertyName] = entry[propertyName];
            }
            destination.PropertyChangedEx += ItemChanged;
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
