using Microsoft.Win32;
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
                            Entries = NetworkData,
                            Filters = Filters
                        };

                        dlg.ShowDialog();
                    },
                    (obj) => NetworkData != null));
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
                            string partialPath = dlg.Text + ".xml";
                            ViewSettings viewSettings = new ViewSettings(dataGridExtended.SaveSettings(),
                                                        new CVSStyle(DataViewSource));
                            GridViewSettingsManager.SaveToXML(viewSettings,
                                                        Consts.DGSettingsPath + partialPath);
                            MainGridStyles.Add(partialPath);
                            SelectedMainGridStyle = partialPath;
                        }
                    },
                    (obj) => NetworkData != null));
            }
        }

        private void LoadViewSettings(string stylename)
        {
            ViewSettings viewSettings = GridViewSettingsManager.LoadFromXML(Consts.DGSettingsPath + stylename);
            dataGridExtended.LoadSettings(viewSettings.dataGridStyle);


            DataViewSource.GroupDescriptions.Clear();
            viewSettings.collectionViewSourceStyle.Apply(DataViewSource);
        }

        public void Close()
        {
            if (networkManager != null)
                networkManager.Close();
        }
    }
}
