using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ZDB.Database;

namespace ZDB
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Entry> data;
        public ObservableCollection<Entry> Data
        {
            get => data;
            set
            {
                if (data != value)
                {
                    data = value;
                    OnPropertyChanged("Data");
                }
            }
        }

        private FilterCollection filters;
        public FilterCollection Filters
        {
            get => filters;
            set
            {
                if (filters != value)
                {
                    filters = value;
                    OnPropertyChanged("Filters");
                }
            }
        }

        private void cvsFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is Entry entry)
            {
                e.Accepted = (entry.Number > 500);
            }
        }

        public MainViewModel(ObservableCollection<Entry> _data)
        {
            Data = _data;
            filters = new FilterCollection();

            filters.CollectionChanged += new NotifyCollectionChangedEventHandler
                (delegate (object sender, NotifyCollectionChangedEventArgs e)
                {
                    if (e.NewItems != null)
                    {
                        foreach (IFilter f in e.NewItems)
                        {
                            // ToDo Unsub
                            f.PropertyChanged += filters.FilterChangeHandler;
                            f.PropertyChanged += FilterRefresh;
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (IFilter f in e.OldItems)
                        {
                            f.PropertyChanged -= filters.FilterChangeHandler;
                            f.PropertyChanged -= FilterRefresh;
                        }
                        FilterRefresh(sender, new PropertyChangedEventArgs("Delete item"));
                    }
                });
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Content c)
            {
                e.Accepted = filters.Filter(c);
            }
        }
        private void FilterRefresh(object sender, PropertyChangedEventArgs e)
        {
            //cvsContents.Refresh();
        }

        
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
