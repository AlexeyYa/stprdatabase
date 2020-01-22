using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace ZDB
{
    public struct ColumnInfo
    {
        public ColumnInfo(DataGridColumn column, int columnID)
        {
            WidthValue = column.Width.DisplayValue;
            WidthType = column.Width.UnitType;
            DisplayIndex = column.DisplayIndex;
            Visibility = column.Visibility;
            ColumnID = columnID;
            ColStyle = column.CellStyle;
        }
        public ColumnInfo(Visibility vis, int dispIdx, double widthVal, DataGridLengthUnitType widthT,
            int columnID, Style colStyle)
        {
            WidthValue = widthVal;
            WidthType = widthT;
            DisplayIndex = dispIdx;
            Visibility = vis;
            ColumnID = columnID;
            ColStyle = colStyle;
        }

        public void Apply(DataGridColumn target)
        {
            target.DisplayIndex = DisplayIndex;
            target.Visibility = Visibility;
            target.Width = new DataGridLength(WidthValue, WidthType);
            target.CellStyle = ColStyle;
        }

        public Style ColStyle;
        public int ColumnID;
        public Visibility Visibility;
        public int DisplayIndex;
        public double WidthValue;
        public DataGridLengthUnitType WidthType;
    }

    class DatagridExtended : DataGrid
    {
        public DatagridExtended() : base()
        {
            var columnStyle = new Style { TargetType = typeof(DataGridColumnHeader) };
            var eventSetter = new EventSetter(MouseRightButtonDownEvent, new MouseButtonEventHandler(HeaderClick));
            columnStyle.Setters.Add(eventSetter);
            ColumnHeaderStyle = columnStyle;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public void LoadSettings(IList<ColumnInfo> CInfo)
        {
            if (CInfo.Count == Columns.Count)
            {
                foreach (var columnInfo in CInfo)
                {
                    int i = columnInfo.ColumnID;
                    columnInfo.Apply(Columns[i]);
                }
            }
        }



        public List<ColumnInfo> SaveSettings()
        {
            List<ColumnInfo> CInfo = new List<ColumnInfo>();
            for (int i = 0; i < Columns.Count(); i++)
            {
                DataGridColumn column = Columns.Where(x => (x.DisplayIndex == i)).First();
                int id = Columns.IndexOf(column);
                ColumnInfo columnInfo = new ColumnInfo(column, id);
                CInfo.Add(columnInfo);
            }
            return CInfo;
        }

        public Style GetRowStyle()
        {
            Style rowStyle = RowStyle;
            return rowStyle;
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
