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
using System.Windows.Controls;
using ZDB.StyleSettings;

namespace ZDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            Parser.Parser.Initialize();

            var dGridInit = new DataGridCustomInit(dGrid);
        }

        /*private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Redo();
        }*/

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DGSettingsManager.SaveToXML(dGrid.SaveSettings(), Consts.DGSettingsPath);
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            dGrid.LoadSettings(DGSettingsManager.LoadFromXML(Consts.DGSettingsPath));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainViewModel.SaveChanges();
        }

        /*private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Undo();
        }*/

    }
}