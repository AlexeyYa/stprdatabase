using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using ZDB.Database;
using ZDB.StyleSettings;

namespace ZDB.MainViewModel
{
    partial class MainViewModelClass
    {
        private RelayCommand addEntryCommand;
        public RelayCommand AddEntryCommand
        {
            get
            {
                return addEntryCommand ??
                    (addEntryCommand = new RelayCommand(obj =>
                    {
                        if (NetworkData.Count > 0)
                            NetworkData.Add(new Entry(NetworkData.Last().Number + 1));
                        else NetworkData.Add(new Entry(1));
                    },
                    (obj) => NetworkData != null));
            }
        }
        private RelayCommand addEntryFromFileCommand;
        public RelayCommand AddEntryFromFileCommand
        {
            get
            {
                return addEntryFromFileCommand ??
                    (addEntryFromFileCommand = new RelayCommand(obj =>
                    {
                        OpenFileDialog dialog = new OpenFileDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            foreach (string path in dialog.FileNames)
                            {
                                Entry entry = Parser.Parser.ProcessFromDocx(path, NetworkData.Last().Number + 1);
                                NetworkData.Add(entry);
                            }
                        }
                    },
                    (obj) => NetworkData != null));
            }
        }

        private RelayCommand removeEntriesCommand;
        public RelayCommand RemoveEntriesCommand
        {
            get
            {
                return removeEntriesCommand ??
                    (removeEntriesCommand = new RelayCommand(obj =>
                    {
                        while (((IList<DataGridCellInfo>)obj).Count() > 0)
                        {
                            var items = (from cell in (IList<DataGridCellInfo>)obj select cell.Item).Distinct().ToList();
                            foreach (Entry item in items)
                            {
                                NetworkData.Remove(item);
                            }
                        }
                    },
                    (obj) => ((IList<DataGridCellInfo>)obj).Count() > 0));
            }
        }

        private RelayCommand serverStartCommand;
        public RelayCommand ServerStartCommand
        {
            get
            {
                return serverStartCommand ??
                    (serverStartCommand = new RelayCommand(obj =>
                    {
                        networkManager.StartServer();
                    },
                    (obj) => true));
            }
        }
        private RelayCommand clientStartCommand;
        public RelayCommand ClientStartCommand
        {
            get
            {
                return clientStartCommand ??
                    (clientStartCommand = new RelayCommand(obj =>
                    {
                        SimpleInputDialog inputDialog = new SimpleInputDialog("Q", 
                            "Введите адрес сервера:", "127.0.0.1");
                        if (inputDialog.ShowDialog() == true)
                        {
                            networkManager.StartClient(NetworkData, inputDialog.Text);
                        }
                    },
                    (obj) => true));
            }
        }
    }
}
