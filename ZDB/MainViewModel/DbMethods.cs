using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using ZDB.Database;

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
                        Data.Add(new Entry(Data.Last().Number + 1));
                    },
                    (obj) => Data != null));
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
                                Entry entry = Parser.Parser.ProcessFromDocx(path, Data.Last().Number + 1);
                                Data.Add(entry);
                            }
                        }
                    },
                    (obj) => Data != null));
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
                                Data.Remove(item);
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
