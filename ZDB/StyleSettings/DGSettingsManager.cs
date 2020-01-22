using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace ZDB.StyleSettings
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
                switch (xNode.Name)
                {
                    case "ColumnsInfo":
                        CInfo = LoadColumns(xNode);
                        break;
                }
            }
            return CInfo;
        }

        private static List<ColumnInfo> LoadColumns(XmlElement parent)
        {
            List<ColumnInfo> CInfo = new List<ColumnInfo>();

            foreach (XmlElement xNode in parent)
            {
                Visibility visibility = Visibility.Visible;
                int DisplayIndex = 0;
                double WidthValue = 40.0;
                DataGridLengthUnitType WidthType = DataGridLengthUnitType.Auto;
                int ColumnID = -1;
                Style ColStyle = new Style();
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
                        case "ColStyle":
                            ColStyle = LoadStyleSetters(childNode);
                            break;
                    }
                }
                if (ColumnID == -1) { break; }
                ColumnInfo column = new ColumnInfo(visibility, DisplayIndex, WidthValue, WidthType, ColumnID, ColStyle);
                CInfo.Add(column);
            }

            return CInfo;
        }

        private static Style LoadStyleSetters(XmlNode parent)
        {
            Style style = new Style();
            foreach (XmlNode StyleSetter in parent)
            {
                switch (StyleSetter.Name)
                {
                    case "TextAlignment":
                        Enum.TryParse(StyleSetter.InnerText, out TextAlignment alignment);
                        Setter textAlignment = new Setter(TextBlock.TextAlignmentProperty,
                            alignment);
                        style.Setters.Add(textAlignment);
                        break;
                    case "FontFamily":
                        Setter fontFamily = new Setter(DataGridCell.FontFamilyProperty,
                            new FontFamily(StyleSetter.InnerText));
                        style.Setters.Add(fontFamily);
                        break;
                    case "FontSize":
                        Double.TryParse(StyleSetter.InnerText, out double fontSizeValue);
                        Setter fontSize = new Setter(DataGridCell.FontSizeProperty,
                            fontSizeValue);
                        style.Setters.Add(fontSize);
                        break;
                    case "Background":
                        Setter background = new Setter(DataGridCell.BackgroundProperty,
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString(StyleSetter.InnerText)));
                        style.Setters.Add(background);
                        break;
                    case "Foreground":
                        Setter foreground = new Setter(DataGridCell.ForegroundProperty,
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString(StyleSetter.InnerText)));
                        style.Setters.Add(foreground);
                        break;
                }
            }
            return style;
        }

        public static void SaveToXML(IEnumerable<ColumnInfo> CInfo, string path)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement xRoot = xDoc.DocumentElement;
            xDoc.InsertBefore(xmlDeclaration, xRoot);
            XmlElement mainElement = xDoc.CreateElement("DGSettings");
            xDoc.AppendChild(mainElement);

            SaveColumns(xDoc, mainElement, CInfo);

            XmlElement element1 = xDoc.CreateElement("RowsInfo");
            mainElement.AppendChild(element1);

            xDoc.Save(path);
        }

        private static void SaveColumns(XmlDocument xDoc, XmlElement parent, IEnumerable<ColumnInfo> CInfo)
        {
            XmlElement element1 = xDoc.CreateElement("ColumnsInfo");
            parent.AppendChild(element1);
            
            foreach (ColumnInfo columnInfo in CInfo)
            {
                XmlElement columnXml = xDoc.CreateElement("Column");
                element1.AppendChild(columnXml);

                XmlElement colIdNode = xDoc.CreateElement("ColumnID");
                XmlText colIdText = xDoc.CreateTextNode(columnInfo.ColumnID.ToString());
                colIdNode.AppendChild(colIdText);
                columnXml.AppendChild(colIdNode);

                XmlElement visibilityNode = xDoc.CreateElement("Visibility");
                XmlText visibilityVal = xDoc.CreateTextNode(columnInfo.Visibility.ToString());
                visibilityNode.AppendChild(visibilityVal);
                columnXml.AppendChild(visibilityNode);

                XmlElement displayIdxNode = xDoc.CreateElement("DisplayIndex");
                XmlText displayIdxVal = xDoc.CreateTextNode(columnInfo.DisplayIndex.ToString());
                displayIdxNode.AppendChild(displayIdxVal);
                columnXml.AppendChild(displayIdxNode);

                XmlElement widthValNode = xDoc.CreateElement("WidthValue");
                XmlText widthVal = xDoc.CreateTextNode(columnInfo.WidthValue.ToString());
                widthValNode.AppendChild(widthVal);
                columnXml.AppendChild(widthValNode);

                XmlElement widthTNode = xDoc.CreateElement("WidthType");
                XmlText widthTVal = xDoc.CreateTextNode(columnInfo.WidthType.ToString());
                widthTNode.AppendChild(widthTVal);
                columnXml.AppendChild(widthTNode);

                if (columnInfo.ColStyle != null)
                {
                    XmlElement colStyleNode = xDoc.CreateElement("ColStyle");
                    columnXml.AppendChild(colStyleNode);

                    foreach (Setter styleSetter in columnInfo.ColStyle.Setters)
                    {
                        XmlElement styleSetterNode = xDoc.CreateElement(styleSetter.Property.ToString());
                        XmlText styleSetterVal = xDoc.CreateTextNode(styleSetter.Value.ToString());
                        styleSetterNode.AppendChild(styleSetterVal);
                        colStyleNode.AppendChild(styleSetterNode);
                    }
                }
            }
        }
    }
}
