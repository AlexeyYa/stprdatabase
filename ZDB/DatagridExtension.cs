using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ZDB
{
    class DatagridExtended :DataGrid
    {
        public DatagridExtended() : base()
        {
            var style = new Style { TargetType = typeof(DataGridColumnHeader) };
            var eventSetter = new EventSetter(MouseRightButtonDownEvent, new MouseButtonEventHandler(HeaderClick));
            style.Setters.Add(eventSetter);
            ColumnHeaderStyle = style;
        }

        private void HeaderClick(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            var visibleColumns = this.Columns.Where(c => c.Visibility == Visibility.Visible).Count();
            foreach (var column in this.Columns)
            {
                var menuItem = new MenuItem
                {
                    Header = column.Header.ToString(),
                    IsChecked = column.Visibility == Visibility.Visible,
                    IsCheckable = true,
                    IsEnabled = visibleColumns > 1 || column.Visibility != Visibility.Visible
                };
                menuItem.Checked += (object a, RoutedEventArgs ea)
                    => column.Visibility = Visibility.Visible;
                menuItem.Unchecked += (object a, RoutedEventArgs ea)
                    => column.Visibility = Visibility.Collapsed;
                menu.Items.Add(menuItem);
            }
            menu.IsOpen = true;
        }
    }
}
