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

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Entity;

using ZDB.MainViewModel;
using ZDB.Database;
using ZDB.Exp;

namespace ZDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICollectionView cvsContents;
        // private Contents _contents;
        // private FilterProcessor _filterProc = new FilterProcessor();
        // private ObservableCollection<Filter> _filters = new ObservableCollection<Filter>();
        //private static FilterCollection _filters = new FilterCollection();
        private static List<DocTemplate> dtList = new List<DocTemplate>();

        private MainViewModelClass mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();

        }

        private void LoadData()
        {
            mainViewModel = new MainViewModelClass();
            this.DataContext = mainViewModel;

            cvsContents = CollectionViewSource.GetDefaultView(dGrid.ItemsSource);
            
            /*_contents = (Contents)this.Resources["ContentsClass"];
            LoadContents(Consts.DatabasePath);
            Logger.Load(_contents);
            _contents.CollectionChanged += new NotifyCollectionChangedEventHandler(Logger.CollectionChanged);
            
            */
            /*
            filterGrid.ItemsSource = _filters;
            _filters.CollectionChanged += new NotifyCollectionChangedEventHandler
                (delegate (object sender, NotifyCollectionChangedEventArgs e)
                {
                    if (e.NewItems != null)
                    {
                        foreach (IFilter f in e.NewItems)
                        {
                            // ToDo Unsub
                            f.PropertyChanged += _filters.FilterChangeHandler;
                            f.PropertyChanged += FilterRefresh;
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (IFilter f in e.OldItems)
                        {
                            f.PropertyChanged -= _filters.FilterChangeHandler;
                            f.PropertyChanged -= FilterRefresh;
                        }
                        FilterRefresh(sender, new PropertyChangedEventArgs("Delete item"));
                    }
                });

            
            */
            if (File.Exists(Consts.TemplatePath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(Consts.TemplatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dtList = (List<DocTemplate>)formatter.Deserialize(stream);
                stream.Close();
            }
        }

        private void LoadContents(string path)
        {
            /*using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.SetDelimiters(";");
                csvParser.ReadLine();
                string[] dateformats = { "dd.MM.yyyy", "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "dd-MM-yyyy", "d-MM-yyyy", "yyyy-MM-dd" };

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();

                    DateTime.TryParseExact(fields[1], dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out DateTime stDate);
                    DateTime.TryParseExact(fields[1], dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out DateTime enDate);
                    DateTime.TryParseExact(fields[1], dateformats,
                                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                out DateTime comDate);
                    Int32.TryParse(fields[19], out int sizecor);
                    Int32.TryParse(fields[9], out int sf);
                    Int32.TryParse(fields[10], out int orig);
                    Int32.TryParse(fields[11], out int cpy);
                    Int32.TryParse(fields[15], out int numer);
                    Int32.TryParse(fields[16], out int scan);
                    Int32.TryParse(fields[17], out int thr);
                    Content x = new Content()
                    {
                        Number = fields[0],
                        StartDate = stDate,
                        CodeType = Convert.ToInt32(fields[2]),
                        User = fields[3],
                        Group = fields[4],
                        Obj = fields[5],
                        DocCode = fields[6],
                        Subs = fields[7],
                        Tasks = fields[8],
                        SizeFormat = sf,
                        NumberOfOriginals = orig,
                        NumberOfCopies = cpy,
                        EndDate = enDate,
                        CompleteDate = comDate,
                        Status = fields[14],
                        Numeration = numer,
                        Scan = scan,
                        Threading = thr,
                        Executor = fields[18],
                        SizeCorFormat = sizecor,
                        Corrections = fields[20]
                    };
                    _contents.Add(x);

                }
            }*/
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Entry c)
            {
                e.Accepted = true;//_filters.Filter(c);
            }
        }

        /*
        private void FilterRefresh(object sender, PropertyChangedEventArgs e)
        {
            cvsContents.Refresh();
        }*/
        /*
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                foreach (string path in dialog.FileNames)
                {
                    Content content = Parser.ProcessFromDocx(path, dtList);
                    content.Number = _contents.GenerateID();
                    _contents.Add(content);
                }
            }
        }*/

        private void TemplateBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                DocTemplate docTemplate = Parser.Template(dialog.FileName);
                dtList.Add(docTemplate);
                Stream stream = new FileStream(Consts.TemplatePath, FileMode.Create, FileAccess.Write, FileShare.None);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, dtList);
                stream.Close();
            }
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ExportWindow
            {
                Owner = Application.Current.MainWindow
            };

            //dlg.Entries = _contents;
            //dlg.Filters = _filters;

            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {

            }
        }

        /*private void AddZ_Click(object sender, RoutedEventArgs e)
        {
            Content c = new Content(_contents.GenerateID());
            _contents.Add(c);
        }*/

        private void AddFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            //_filters.Add(new FilterString("Obj", FilterOperation.GREATEREQUAL, ""));
        }

        /*private void RemoveZ_Click(object sender, RoutedEventArgs e)
        {
            _contents.Remove((Content)dGrid.SelectedItem);
        }*/

        /*private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Redo();
        }*/

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DGSettingsManager.SaveToXML(dGrid.SaveSettings(), Consts.ColumnSettingsPath);
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            dGrid.LoadSettings(DGSettingsManager.LoadFromXML(Consts.ColumnSettingsPath));
        }

        /*private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Undo();
        }*/

    }
}