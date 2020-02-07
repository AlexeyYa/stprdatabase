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
using System.Windows;

using ZDB.MainViewModel;

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

        /// <summary>
        /// Put initialization here
        /// </summary>
        private void LoadData()
        {
            mainViewModel = new MainViewModelClass(dGrid);
            this.DataContext = mainViewModel;
            Parser.Parser.Initialize();
        }

        /*private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Redo();
        }*/

        /// <summary>
        /// Save db on quit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            mainViewModel.Close();
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        /*private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger.Undo();
        }*/

    }
}