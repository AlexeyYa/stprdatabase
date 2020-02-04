﻿using Microsoft.Win32;
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
                        NetworkData.Add(new Entry(NetworkData.Last().Number + 1));
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

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
