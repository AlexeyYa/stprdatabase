/*

Copyright 2019 Yamborisov Alexey

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
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
using System.Xml.Serialization;

namespace ZDB
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private ExportViewModel exportVM;
        public ExportViewModel ExportVM { get { return exportVM; } }

        public ExportWindow()
        {
            InitializeComponent();
            exportVM = new ExportViewModel();
            DataContext = exportVM;   
        }

        private Contents contents;
        public Contents Contents { get => contents; set => contents = value; }

        private FilterCollection filters;
        internal FilterCollection Filters { get => filters; set => filters = value; }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * List<string> fields = new List<string>();
            foreach (FieldsEntryViewmodel f in exportVM.CurrentSetting.Columns)
            {
                fields.Add(f.Field);
            }

            List<string> parts = new List<string>();
            foreach (PartitionsEntryViewmodel p in exportVM.CurrentSetting.Partitions)
            {
                parts.Add(p.Field);
            }

            List<Tuple<string, bool>> sortings = new List<Tuple<string, bool>>();
            foreach (SortingEntryViewmodel s in exportVM.CurrentSetting.SortBy)
            {
                sortings.Add(new Tuple<string, bool>(s.Field, s.Ascending));
            }
            string orderQuery = String.Empty;
            bool first = true;
            foreach (Tuple<string, bool> t in sortings)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    orderQuery += ", ";
                }
                if (t.Item2)
                {
                    orderQuery += t.Item1 + " ASC";
                }
                else
                {
                    orderQuery += t.Item1 + " DESC";
                }
            }

            var sortedContents = Contents.OrderBy(orderQuery);*/
            //Export.ExportCVS(sortedContents, Filters, fields, 
            //    sortings, parts, exportVM.SavePath);
            Export.ExportMain(Contents, Filters, exportVM.CurrentSetting, exportVM.SavePath);
        }
    }
    
    public class FieldsEntryViewmodel : INotifyPropertyChanged
    {
        public FieldsEntryViewmodel() { }
        public FieldsEntryViewmodel(string entry)
        {
            Field = entry;
        }

        private string field;
        public string Field
        {
            get { return field; }
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged("Field");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum PARTITION_TYPE
    {
        SAME_PAGE,
        SAME_DIRECTORY
    }

    public class PartitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(parameter is string parameterString))
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return DependencyProperty.UnsetValue;
            }

            object parameterValue = Enum.Parse(value.GetType(), parameterString);
            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(parameter is string parameterString))
            {
                return DependencyProperty.UnsetValue;
            }
            return Enum.Parse(targetType, parameterString);
        }
    }
    public class PartitionsEntryViewmodel : INotifyPropertyChanged
    {
        public PartitionsEntryViewmodel() { }
        public PartitionsEntryViewmodel(string entry, PARTITION_TYPE p=PARTITION_TYPE.SAME_DIRECTORY)
        {
            Field = entry;
            Partition = p;
        }

        private string field;
        public string Field
        {
            get { return field; }
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged("Field");
                }
            }
        }

        private PARTITION_TYPE partition;
        public PARTITION_TYPE Partition
        {
            get { return partition; }
            set
            {
                if (partition != value)
                {
                    partition = value;
                    OnPropertyChanged("Partition");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    
    public class SortingEntryViewmodel : INotifyPropertyChanged
    {
        public SortingEntryViewmodel() { }
        public SortingEntryViewmodel(string entry, bool isAscending)
        {
            Field = entry;
            Ascending = isAscending;
        }

        private string field;
        public string Field
        {
            get { return field; }
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged("Field");
                }
            }
        }

        private bool ascending;
        public bool Ascending
        {
            get { return ascending; }
            set
            {
                if (ascending != value)
                {
                    ascending = value;
                    OnPropertyChanged("Ascending");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    [Serializable]
    public class ExportSetting : INotifyPropertyChanged
    {
        public ExportSetting(string _name, ObservableCollection<FieldsEntryViewmodel> _c, ObservableCollection<SortingEntryViewmodel> _s, ObservableCollection<PartitionsEntryViewmodel> _p) {
            Name = _name;
            Columns = _c;
            SortBy = _s;
            Partitions = _p;
        }

        public ExportSetting() { }

        public string Name { get; set; }
        private ObservableCollection<FieldsEntryViewmodel> columns;
        public ObservableCollection<FieldsEntryViewmodel> Columns
        {
            get { return columns; }
            set
            {
                if (columns != value)
                {
                    columns = value;
                    OnPropertyChanged("Columns");
                }
            }
        }
        private ObservableCollection<PartitionsEntryViewmodel> partitions;
        public ObservableCollection<PartitionsEntryViewmodel> Partitions
        {
            get { return partitions; }
            set
            {
                if (partitions != value)
                {
                    partitions = value;
                    OnPropertyChanged("Partitions");
                }
            }
        }
        private ObservableCollection<SortingEntryViewmodel> sortBy;
        public ObservableCollection<SortingEntryViewmodel> SortBy
        {
            get { return sortBy; }
            set
            {
                if (sortBy != value)
                {
                    sortBy = value;
                    OnPropertyChanged("SortBy");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    
    public class ExportViewModel : INotifyPropertyChanged
    {
        public ExportViewModel()
        {
            LoadExportSettings();
            currentSetting = new ExportSetting("current", new ObservableCollection<FieldsEntryViewmodel>(),
                                               new ObservableCollection<SortingEntryViewmodel>(),
                                               new ObservableCollection<PartitionsEntryViewmodel>());
            InputDialogVisibility = false;
        }

        private string savePath;
        public string SavePath
        {
            get { return savePath; }
            set
            {
                if (savePath != value)
                {
                    savePath = value;
                    OnPropertyChanged("savePath");
                }
            }
        }
        private RelayCommand savePathCommand;
        public RelayCommand SavePathCommand
        {
            get
            {
                return savePathCommand ??
                    (savePathCommand = new RelayCommand(obj =>
                    {
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            ValidateNames = false,
                            CheckFileExists = false,
                            CheckPathExists = true,
                            FileName = "Выбор папки"
                        };
                        if (dialog.ShowDialog() == true)
                        {
                            string path = System.IO.Path.GetDirectoryName(dialog.FileName);
                            this.SavePath = path;
                        }
                    }));
            }
        }

        /// <summary>
        /// Settings collection and settings management
        /// </summary>
        private ExportSetting currentSetting;
        public ExportSetting CurrentSetting
        {
            get { return currentSetting; }
            set
            {
                if (currentSetting != value)
                {
                    currentSetting = value;
                    OnPropertyChanged("ExportSettings");
                }
            }
        }
        private ExportSetting selectedSetting;
        public ExportSetting SelectedSetting
        {
            get { return selectedSetting; }
            set
            {
                if (selectedSetting != value)
                {
                    selectedSetting = value;
                    if (value != null)
                    {
                        CurrentSetting.Name = value.Name;
                        CurrentSetting.Columns = new ObservableCollection<FieldsEntryViewmodel>(value.Columns);
                        CurrentSetting.SortBy = new ObservableCollection<SortingEntryViewmodel>(value.SortBy);
                        CurrentSetting.Partitions = new ObservableCollection<PartitionsEntryViewmodel>(value.Partitions);
                    }
                    OnPropertyChanged("SelectedSetting");
                }
            }
        }
        private ObservableCollection<ExportSetting> exportSettings;
        public ObservableCollection<ExportSetting> ExportSettings {
            get { return exportSettings; }
            set
            {
                if (exportSettings != value)
                {
                    exportSettings = value;
                    OnPropertyChanged("ExportSettings");
                }
            }
        }
        private void LoadExportSettings()
        {
            if (File.Exists("exportsettings.xml"))
            {
                using (FileStream fs = new FileStream("exportsettings.xml", FileMode.OpenOrCreate))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<ExportSetting>));
                    ExportSettings = (ObservableCollection<ExportSetting>)formatter.Deserialize(fs);
                }
            }
            else
            {
                ExportSettings = new ObservableCollection<ExportSetting>();
            }
        }
        private void SaveExportSettings()
        {
            using (FileStream fs = new FileStream("exportsettings.xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<ExportSetting>));
                formatter.Serialize(fs, ExportSettings);
            }
        }
        private RelayCommand saveSettingCommand;
        public RelayCommand SaveSettingCommand
        {
            get
            {
                return saveSettingCommand ??
                    (saveSettingCommand = new RelayCommand(obj =>
                    {
                        InputDialogVisibility = true;
                    }));
            }
        }
        private RelayCommand removeSettingCommand;
        public RelayCommand RemoveSettingCommand
        {
            get
            {
                return removeSettingCommand ??
                    (removeSettingCommand = new RelayCommand(obj =>
                    {
                        ExportSettings.Remove(SelectedSetting);
                        SelectedSetting = null;
                    },
                    (obj) => SelectedSetting != null));
            }
        }
        private RelayCommand restoreSettingCommand;
        public RelayCommand RestoreSettingCommand
        {
            get
            {
                return restoreSettingCommand ??
                    (restoreSettingCommand = new RelayCommand(obj =>
                    {
                        CurrentSetting.Name = SelectedSetting.Name;
                        CurrentSetting.Columns = new ObservableCollection<FieldsEntryViewmodel>(SelectedSetting.Columns);
                        CurrentSetting.SortBy = new ObservableCollection<SortingEntryViewmodel>(SelectedSetting.SortBy);
                        CurrentSetting.Partitions = new ObservableCollection<PartitionsEntryViewmodel>(SelectedSetting.Partitions);
                    },
                    (obj) => SelectedSetting != null));
            }
        }
        ///// <summary>
        ///// Export Progress bar
        ///// </summary>
        //private bool progressBarVisibility;
        //public bool ProgressBarVisibility
        //{
        //    get { return progressBarVisibility; }
        //    set
        //    {
        //        if (progressBarVisibility != value)
        //        {
        //            progressBarVisibility = value;
        //            OnPropertyChanged("ProgressBarVisibility");
        //        }
        //    }
        //}
        //private double currentProgress;
        //public double CurrentProgress
        //{
        //    get { return currentProgress; }
        //    set
        //    {
        //        if (currentProgress != value)
        //        {
        //            currentProgress = value;
        //            OnPropertyChanged("CurrentProgress");
        //        }
        //    }
        //}
        /// <summary>
        /// Settings name input
        /// </summary>
        private bool inputDialogVisibility;
        public bool InputDialogVisibility
        {
            get { return inputDialogVisibility; }
            set
            {
                if (inputDialogVisibility != value)
                {
                    inputDialogVisibility = value;
                    OnPropertyChanged("InputDialogVisibility");
                }
            }
        }
        private RelayCommand cancelNewSettingCommand;
        public RelayCommand CancelNewSettingCommand
        {
            get
            {
                return cancelNewSettingCommand ??
                    (cancelNewSettingCommand = new RelayCommand(obj =>
                    {
                        InputDialogVisibility = false;
                    }));
            }
        }
        private RelayCommand confirmNewSettingCommand;
        public RelayCommand ConfirmNewSettingCommand
        {
            get
            {
                return confirmNewSettingCommand ??
                    (confirmNewSettingCommand = new RelayCommand(obj =>
                    {
                        ExportSetting newSetting = new ExportSetting(CurrentSetting.Name, CurrentSetting.Columns,
                                                           CurrentSetting.SortBy, CurrentSetting.Partitions);
                        ExportSettings.Add(newSetting);
                        SelectedSetting = newSetting;
                        SaveExportSettings();
                        InputDialogVisibility = false;
                    }));
            }
        }

        /// <summary>
        /// Dynamic controls commands
        /// </summary>
        private RelayCommand addFieldCommand;
        public RelayCommand AddFieldCommand
        {
            get
            {
                return addFieldCommand ??
                    (addFieldCommand = new RelayCommand(obj =>
                    {
                        FieldsEntryViewmodel fieldsEntry = new FieldsEntryViewmodel("Obj");
                        CurrentSetting.Columns.Add(fieldsEntry);
                    }));
            }
        }
        private RelayCommand removeFieldCommand;
        public RelayCommand RemoveFieldCommand
        {
            get
            {
                return removeFieldCommand ??
                    (removeFieldCommand = new RelayCommand(obj =>
                    {
                        CurrentSetting.Columns.Remove(CurrentSetting.Columns.Last());
                    },
                    (obj) => CurrentSetting.Columns.Count > 0));
            }
        }
        
        private RelayCommand addPartitionCommand;
        public RelayCommand AddPartitionCommand
        {
            get
            {
                return addPartitionCommand ??
                    (addPartitionCommand = new RelayCommand(obj =>
                    {
                        PartitionsEntryViewmodel fieldsEntry = new PartitionsEntryViewmodel("Obj");
                        CurrentSetting.Partitions.Add(fieldsEntry);
                    }));
            }
        }
        private RelayCommand removePartitionCommand;
        public RelayCommand RemovePartitionCommand
        {
            get
            {
                return removePartitionCommand ??
                    (removePartitionCommand = new RelayCommand(obj =>
                    {
                        CurrentSetting.Partitions.Remove(CurrentSetting.Partitions.Last());
                    },
                    (obj) => CurrentSetting.Partitions.Count > 0));
            }
        }
        
        private RelayCommand addSortingCommand;
        public RelayCommand AddSortingCommand
        {
            get
            {
                return addSortingCommand ??
                    (addSortingCommand = new RelayCommand(obj =>
                    {
                        SortingEntryViewmodel sortEntry = new SortingEntryViewmodel("Obj", true);
                        CurrentSetting.SortBy.Add(sortEntry);
                    }));
            }
        }
        private RelayCommand removeSortingCommand;
        public RelayCommand RemoveSortingCommand
        {
            get
            {
                return removeSortingCommand ??
                    (removeSortingCommand = new RelayCommand(obj =>
                    {
                        CurrentSetting.SortBy.Remove(CurrentSetting.SortBy.Last());
                    },
                    (obj) => CurrentSetting.SortBy.Count > 0));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}