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
using System.Xml;

namespace ZDB
{
    public static class DGSettingsManager
    {
        public static List<ColumnInfo> LoadFromXML(string path)
        {
            List<ColumnInfo> CInfo = new List<ColumnInfo>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlElement xNode in xRoot)
            {
                Visibility visibility = Visibility.Visible;
                int DisplayIndex = 0;
                double WidthValue = 40.0;
                DataGridLengthUnitType WidthType = DataGridLengthUnitType.Auto;
                int ColumnID = -1;
                foreach (XmlNode childNode in xNode.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "ColumnID":
                            Int32.TryParse(childNode.InnerText, out ColumnID);
                            break;
                        case "Visibility":
                            Enum.TryParse(childNode.InnerText, out visibility);
                            break;
                        case "DisplayIndex":
                            Int32.TryParse(childNode.InnerText, out DisplayIndex);
                            break;
                        case "WidthValue":
                            Double.TryParse(childNode.InnerText, out WidthValue);
                            break;
                        case "WidthType":
                            Enum.TryParse(childNode.InnerText, out WidthType);
                            break;
                    }
                }
                if (ColumnID == -1) { break; }
                ColumnInfo column = new ColumnInfo(visibility, DisplayIndex, WidthValue, WidthType, ColumnID);
                CInfo.Add(column);
            }
            return CInfo;
        }

        public static void SaveToXML(IEnumerable<ColumnInfo> CInfo, string path)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement xRoot = xDoc.DocumentElement;
            xDoc.InsertBefore(xmlDeclaration, xRoot);
            XmlElement element1 = xDoc.CreateElement("ColumnsInfo");
            xDoc.AppendChild(element1);
            foreach (ColumnInfo columnInfo in CInfo)
            {
                XmlElement columnXml = xDoc.CreateElement("Column");
                element1.AppendChild(columnXml);

                XmlElement colIdNode = xDoc.CreateElement("ColumnID");
                XmlText colIdText = xDoc.CreateTextNode(columnInfo.ColumnID.ToString());
                colIdNode.AppendChild(colIdText);
                columnXml.AppendChild(colIdNode);

                XmlElement visibilityNode = xDoc.CreateElement("Visibility");
                XmlText visibilityText = xDoc.CreateTextNode(columnInfo.Visibility.ToString());
                visibilityNode.AppendChild(visibilityText);
                columnXml.AppendChild(visibilityNode);

                XmlElement displayIdxNode = xDoc.CreateElement("DisplayIndex");
                XmlText displayIdxText = xDoc.CreateTextNode(columnInfo.DisplayIndex.ToString());
                displayIdxNode.AppendChild(displayIdxText);
                columnXml.AppendChild(displayIdxNode);

                XmlElement widthValNode = xDoc.CreateElement("WidthValue");
                XmlText widthValText = xDoc.CreateTextNode(columnInfo.WidthValue.ToString());
                widthValNode.AppendChild(widthValText);
                columnXml.AppendChild(widthValNode);

                XmlElement widthTNode = xDoc.CreateElement("WidthType");
                XmlText widthTText = xDoc.CreateTextNode(columnInfo.WidthType.ToString());
                widthTNode.AppendChild(widthTText);
                columnXml.AppendChild(widthTNode);
            }
            xDoc.Save(path);
        }
    }

    public struct ColumnInfo
    {
        public ColumnInfo(DataGridColumn column, int columnID)
        {
            WidthValue = column.Width.DisplayValue;
            WidthType = column.Width.UnitType;
            DisplayIndex = column.DisplayIndex;
            Visibility = column.Visibility;
            ColumnID = columnID;
        }
        public ColumnInfo(Visibility vis, int dispIdx, double widthVal, DataGridLengthUnitType widthT, int columnID)
        {
            WidthValue = widthVal;
            WidthType = widthT;
            DisplayIndex = dispIdx;
            Visibility = vis;
            ColumnID = columnID;
        }

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
            var style = new Style { TargetType = typeof(DataGridColumnHeader) };
            var eventSetter = new EventSetter(MouseRightButtonDownEvent, new MouseButtonEventHandler(HeaderClick));
            style.Setters.Add(eventSetter);
            ColumnHeaderStyle = style;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public void LoadSettings(IList<ColumnInfo> CInfo)
        {
            if (CInfo.Count == Columns.Count)
            {
                foreach (var column in CInfo)
                {
                    int i = column.ColumnID;
                    Columns[i].DisplayIndex = column.DisplayIndex;
                    Columns[i].Visibility = column.Visibility;
                    Columns[i].Width = new DataGridLength(column.WidthValue, column.WidthType);
                }
                /*for (int i = 0; i < CInfo.Count(); i++)
                {

                    Columns[i].DisplayIndex = CInfo[i].DisplayIndex;
                    Columns[i].Visibility = CInfo[i].Visibility;
                    Columns[i].Width = new DataGridLength(CInfo[i].WidthValue, CInfo[i].WidthType);
                }*/
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
