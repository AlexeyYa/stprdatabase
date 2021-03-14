using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Windows.Data;
using ZDB.Database;
using ZDB.Network;
using ZDB.StyleSettings;

namespace ZDB.MainViewModel
{
    partial class MainViewModelClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly DataGridExtended dataGridExtended;

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
        private NetworkCollection networkData;
        public NetworkCollection NetworkData
        {
            get => networkData;
            set
            {
                if (networkData != value)
                {
                    networkData = value;
                    OnPropertyChanged("NetworkData");
                }
            }
        }
        private readonly NetworkManager networkManager;
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

        // Initialized on get from directory
        private ObservableCollection<string> mainGridStyles = null;
        public ObservableCollection<string> MainGridStyles
        {
            get
            {
                if (mainGridStyles == null)
                {
                    mainGridStyles = new ObservableCollection<string>();
                    foreach (string filename in Directory.GetFiles(Consts.DGSettingsPath))
                    {
                        mainGridStyles.Add(filename.Replace(Consts.DGSettingsPath, ""));
                    }
                }
                return mainGridStyles;
            }
        }

        public string selectedMainGridStyle;
        public string SelectedMainGridStyle
        {
            get => selectedMainGridStyle;
            set
            {
                if (selectedMainGridStyle != value)
                {
                    selectedMainGridStyle = value;
                    LoadViewSettings(value);
                    OnPropertyChanged("SelectedMainGridStyle");
                }
            }
        }

        public MainViewModelClass(DataGridExtended dGrid)
        {
            // Loading DB
            NetworkData = new NetworkCollection();
            networkManager = new NetworkManager();

            // Setting up Filters
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

            // Setting up ViewSource
            DataViewSource = new CollectionViewSource { Source = NetworkData };
            DataViewSource.Filter += FilterHandler;

            // Init DataGrid
            dataGridExtended = dGrid;
            var dGridInit = new DataGridCustomInit(dataGridExtended);
            if (File.Exists(Consts.DGSettingsPath + Properties.Settings.Default.defaultMainGridSetting))
            {
                SelectedMainGridStyle = Properties.Settings.Default.defaultMainGridSetting;
            }

            

            // Grouping sample
            // DataViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            // DataViewSource.GroupDescriptions.Add(new PropertyGroupDescription("StartDate", new DateToQuartersConverter()));
            // DataViewSource.GroupDescriptions.Add(new PropertyGroupDescription("StartDate", new DateToMonthsConverter()));
        }

        


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
