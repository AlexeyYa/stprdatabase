using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                        Data.Add(new Entry());
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
                                //Content content = Parser.ProcessFromDocx(path, dtList);
                                //Data.Add(content);
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

        private RelayCommand exportCommand;
        public RelayCommand ExportCommand
        {
            get
            {
                return exportCommand ??
                    (exportCommand = new RelayCommand(obj =>
                    {
                        
                    },
                    (obj) => Data != null));
            }
        }
    }
}
