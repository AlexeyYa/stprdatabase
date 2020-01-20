using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ZDB.Database;

namespace ZDB.MainViewModel
{
    partial class MainViewModelClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DatabaseContext db;

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
        private CollectionViewSource dataViewSource;
        public CollectionViewSource DataViewSource
        {
            get => dataViewSource;
            set
            {
                if (dataViewSource != value)
                {
                    dataViewSource = value;
                    OnPropertyChanged("DataViewSource");
                }
            }
        }

        public MainViewModelClass()
        {
            db = new DatabaseContext();
            db.Entries.Load();
            Data = db.Entries.Local;

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

            DataViewSource = new CollectionViewSource();
            DataViewSource.Source = Data;
            DataViewSource.Filter += FilterHandler;
        }

        
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
