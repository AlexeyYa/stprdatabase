using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ZDB
{
    /// <summary>
    /// Interaction logic for SimpleInputDialog.xaml
    /// </summary>
    public partial class SimpleInputDialog : Window
    {
        public SimpleInputDialog(string description="Введите название:", 
                            string defaultAnswer="", string imgType="")
        {
            InitializeComponent();
            lblDescription.Content = description;
            tbInput.Text = defaultAnswer;
            switch (imgType)
            {
                case "Q":
                    imgDescription.Margin = new Thickness(20,0,20,0);
                    break;
                default:
                    break;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            tbInput.SelectAll();
            tbInput.Focus();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (Text != String.Empty)
                this.DialogResult = true;
        }

        public string Text { get { return tbInput.Text; } }
    }
}
