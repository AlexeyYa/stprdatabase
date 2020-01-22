using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZDB.Exp;

namespace ZDB.MainViewModel
{
    partial class MainViewModelClass
    {
        private RelayCommand addTemplateCommand;
        public RelayCommand AddTemplateCommand
        {
            get
            {
                return addTemplateCommand ??
                    (addTemplateCommand = new RelayCommand(obj =>
                    {
                        OpenFileDialog dialog = new OpenFileDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            Parser.Parser.AddTemplate(dialog.FileName);
                        }
                    }));
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
                        var dlg = new ExportWindow
                        {
                            Owner = Application.Current.MainWindow,
                            Entries = Data,
                            Filters = Filters
                        };

                        dlg.ShowDialog();
                    },
                    (obj) => Data != null));
            }
        }
    }
}
