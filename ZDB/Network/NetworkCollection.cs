using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDB.Database;

namespace ZDB.Network
{
    class NetworkCollection : ObservableCollection<Entry>
    {
        private bool isServer = true;
        private ObservableCollection<Entry> dbCollection;
        int saveCounter = 0;

        public NetworkCollection(bool isServer, ObservableCollection<Entry> dbCollection=null)
        {
            this.isServer = isServer;

            if (isServer)
            {
                this.dbCollection = dbCollection;
                foreach (var entry in dbCollection)
                {
                    this.Add(entry.Copy());
                }
            } else
            {
                throw new NotImplementedException();
            }

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
                if (isServer)
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
                } else
                {
                    // serialize sender and e then send
                    throw new NotImplementedException();
                }
            }
        }

        private void ItemChangedRecieved(object sender, PropertyChangedExtendedEventArgs e)
        {
            if (sender is Entry entry)
            {
                if (isServer)
                {
                    Entry destination = this.First(r => r.Number == entry.Number);
                    if (destination[e.PropertyName].ToString() == e.OldValue)
                    {
                        destination[e.PropertyName] = entry[e.PropertyName];
                    }
                    else
                    {
                        throw new MemberAccessException("Error: different initial values");
                    }


                }
                else
                {
                    // serialize sender and e then send
                    throw new NotImplementedException();
                }
            }
        }

        // ADD MUTEX
        // Server - processing sent data and user input, send responce with id number
        // Client - sending data to server
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (isServer)
            {
                if (e.NewItems != null)
                {
                    foreach (Entry entry in e.NewItems)
                    {
                        if (dbCollection.Last().Number >= entry.Number)
                        {
                            entry.Number = dbCollection.Last().Number + 1;
                        }
                        dbCollection.Add(entry);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (Entry entry in e.OldItems)
                    {
                        dbCollection.Remove(dbCollection.First(r => r.Number == entry.Number));
                    }
                }
            } else
            {
                throw new NotImplementedException();
            }
        }

        private void ItemsCollectionChangedRecieved(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (isServer)
            {
                if (e.NewItems != null)
                {
                    foreach (Entry entry in e.NewItems)
                    {
                        if (this.Last().Number >= entry.Number)
                        {
                            entry.Number = this.Last().Number + 1;
                        }
                        this.Add(entry);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (Entry entry in e.OldItems)
                    {
                        this.Remove(this.First(r => r.Number == entry.Number));
                    }
                }
            }
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
