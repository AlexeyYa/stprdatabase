using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
