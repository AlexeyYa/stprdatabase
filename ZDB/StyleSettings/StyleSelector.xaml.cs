using System.Windows;
using ZDB.StyleSettings;

namespace ZDB
{
    /// <summary>
    /// Interaction logic for StyleSelector.xaml
    /// </summary>
    public partial class StyleSelector : Window
    {
        private StyleSelectorMVVM vm;

        public StyleSelector()
        {
            InitializeComponent();
            vm = new StyleSelectorMVVM();
        }
    }
}
