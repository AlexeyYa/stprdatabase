using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ZDB.StyleSettings;

namespace ZDB
{
    /// <summary>
    /// Interaction logic for StyleSelector.xaml
    /// </summary>
    public partial class StyleSelector : Window
    {
        private StyleSelectorMVVM vm;

        List<string> fonts;
        List<double> fontSizes;
        List<string> alignmentsList;
        List<string> vertAlignmentsList;

        public StyleSelector(IList<DataGridCellInfo> selectedCells)
        {
            FillLists();
            InitializeComponent();
        
            Binding dict = new Binding();
            dict.Source = alignmentsList;

            cmbAlignment.SetBinding(ComboBox.ItemsSourceProperty, dict);

            Binding dict1 = new Binding();
            dict1.Source = vertAlignmentsList;

            cmbVertAlignment.SetBinding(ComboBox.ItemsSourceProperty, dict1);

            Binding dict2 = new Binding();
            dict2.Source = fonts;

            cmbFont.SetBinding(ComboBox.ItemsSourceProperty, dict2);

            Binding dict3 = new Binding();
            dict3.Source = fontSizes;

            cmbFSize.SetBinding(ComboBox.ItemsSourceProperty, dict3);
            vm = new StyleSelectorMVVM(selectedCells);
            this.DataContext = vm;
        }

        private void FillLists()
        {
            fonts = new List<string>();
            fontSizes = new List<double>();

            foreach (FontFamily font in System.Drawing.FontFamily.Families)
            {
                fonts.Add(font.Name);
            }

            for (double i = 10; i < 50; i++)
            {
                fontSizes.Add(i);
            }

            alignmentsList = new List<string>
            {
                "Left", "Center", "Right", "Justify"
            };

            vertAlignmentsList = new List<string>
            {
                "Top", "Center", "Bottom", "Stretch"
            };
        }
    }
}
