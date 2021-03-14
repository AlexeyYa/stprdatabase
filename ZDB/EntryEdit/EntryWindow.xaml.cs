using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZDB.Database;

namespace ZDB.EntryEdit
{
    /// <summary>
    /// Interaction logic for EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Entry editedEntry;
        Entry EditedEntry
        {
            get => editedEntry;
            set
            {
                if (editedEntry != value)
                {
                    editedEntry = value;
                    OnPropertyChanged("EditedEntry");
                }
            }
        }

        public EntryWindow(Entry entry)
        {
            EditedEntry = entry;
            InitializeComponent();
            this.DataContext = EditedEntry;
        }

        public Entry GetEntry()
        {
            return EditedEntry;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
