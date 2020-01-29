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
using ZDB.StyleSettings;

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

        private RelayCommand addViewCommand;
        public RelayCommand AddViewCommand
        {
            get
            {
                return addViewCommand ??
                    (addViewCommand = new RelayCommand(obj =>
                    {
                        SimpleInputDialog dlg = new SimpleInputDialog("Q");

                        if (dlg.ShowDialog() == true)
                        {
                            string partialPath = Consts.DGSettingsPath + dlg.Text + ".xml";
                            DGSettingsManager.SaveToXML(dataGridExtended.SaveSettings(),
                                                        Consts.DGSettingsPath + partialPath);
                            MainGridStyles.Add(partialPath);
                            SelectedMainGridStyle = partialPath;
                        }
                    },
                    (obj) => Data != null));
            }
        }
    }
}
