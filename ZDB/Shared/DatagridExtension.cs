using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace ZDB
{
    // Style data for DataGridExtended
    public struct DGEStyle
    {
        public DGEStyle(IList<ColumnInfo> ColumnInfo, Style RowStyle, int FrozenColumnCount)
        {
            CInfo = ColumnInfo;
            rowStyle = RowStyle;
            frozenColumnCount = FrozenColumnCount;
        }

        public IList<ColumnInfo> CInfo;
        public Style rowStyle;
        public int frozenColumnCount;
    }

    // Column style data
    public struct ColumnInfo
    {
        public ColumnInfo(DataGridColumn column, int columnID)
        {
            WidthValue = column.Width.DisplayValue;
            WidthType = column.Width.UnitType;
            DisplayIndex = column.DisplayIndex;
            Visibility = column.Visibility;
            ColumnHeader = column.Header.ToString();
            ColStyle = column.CellStyle;
            if (column is DataGridTextColumn textColumn)
                StringFormat = textColumn.Binding.StringFormat;
            else StringFormat = String.Empty;
        }
        public ColumnInfo(Visibility vis, int dispIdx, double widthVal, DataGridLengthUnitType widthT,
            string columnHeader, Style colStyle, string colStringFormat)
        {
            WidthValue = widthVal;
            WidthType = widthT;
            DisplayIndex = dispIdx;
            Visibility = vis;
            ColumnHeader = columnHeader;
            ColStyle = colStyle;
            StringFormat = colStringFormat;
        }

        public void Apply(DataGridColumn target)
        {
            target.DisplayIndex = DisplayIndex;
            target.Visibility = Visibility;
            target.Width = new DataGridLength(WidthValue, WidthType);
            target.CellStyle = ColStyle;
            if (StringFormat != null && target is DataGridTextColumn textColumn)
            {
                Binding oldBinding = textColumn.Binding as Binding;
                Binding binding = new Binding(oldBinding.Path.Path);

                binding.Mode = oldBinding.Mode;
                binding.ConverterCulture = oldBinding.ConverterCulture;
                binding.UpdateSourceTrigger = oldBinding.UpdateSourceTrigger;

                binding.StringFormat = StringFormat;

                textColumn.Binding = binding;
            }
        }

        public Style ColStyle;
        public string ColumnHeader;
        public Visibility Visibility;
        public int DisplayIndex;
        public double WidthValue;
        public DataGridLengthUnitType WidthType;
        public string StringFormat;
    }

    class DataGridExtended : DataGrid
    {
        public DataGridExtended() : base()
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

        public void LoadSettings(DGEStyle DGEStyleSettings)
        {
            foreach (var columnInfo in DGEStyleSettings.CInfo)
            {
                string Header = columnInfo.ColumnHeader;
                // Applying to column with same header
                columnInfo.Apply(Columns.Where(x => x.Header.ToString() == Header).First());
            }
            RowStyle = DGEStyleSettings.rowStyle;
            FrozenColumnCount = DGEStyleSettings.frozenColumnCount;
        }



        public DGEStyle SaveSettings()
        {
            List<ColumnInfo> CInfo = new List<ColumnInfo>();
            for (int i = 0; i < Columns.Count(); i++)
            {
                DataGridColumn column = Columns.Where(x => (x.DisplayIndex == i)).First();
                int id = Columns.IndexOf(column);
                ColumnInfo columnInfo = new ColumnInfo(column, id);
                CInfo.Add(columnInfo);
            }
            return new DGEStyle(CInfo, RowStyle, FrozenColumnCount);
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
