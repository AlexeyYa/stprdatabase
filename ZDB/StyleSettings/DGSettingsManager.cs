using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

namespace ZDB.StyleSettings
{

    public static class DGSettingsManager
    {
        public static DGEStyle LoadFromXML(string path)
        {
            List<ColumnInfo> CInfo = new List<ColumnInfo>();
            Style rowStyle = new Style();

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
                    case "RowStyle":
                        rowStyle = LoadStyleSetters(xNode);
                        break;
                }
            }
            return new DGEStyle(CInfo, rowStyle);
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

        public static void SaveToXML(DGEStyle DGEStyleSettings, string path)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement xRoot = xDoc.DocumentElement;
            xDoc.InsertBefore(xmlDeclaration, xRoot);
            XmlElement mainElement = xDoc.CreateElement("DGSettings");
            xDoc.AppendChild(mainElement);

            SaveColumns(xDoc, mainElement, DGEStyleSettings.CInfo);

            XmlElement rowStyleNode = xDoc.CreateElement("RowStyle");
            mainElement.AppendChild(rowStyleNode);

            SaveStyle(xDoc, rowStyleNode, DGEStyleSettings.rowStyle);

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

                    SaveSetters(xDoc, colStyleNode, columnInfo.ColStyle.Setters);
                }
            }
        }

        private static void SaveStyle(XmlDocument xDoc, XmlElement parent, Style style) {


            XmlElement styleNode = xDoc.CreateElement("Style");
            parent.AppendChild(styleNode);
            SaveSetters(xDoc, styleNode, style.Setters);

            foreach (var trig in style.Triggers)
            {
                if (trig is DataTrigger dataTrigger)
                {
                    XmlElement dtNode = xDoc.CreateElement("DataTrigger");
                    parent.AppendChild(dtNode);

                    XmlElement bindingNode = xDoc.CreateElement("Binding");
                    Binding binding = dataTrigger.Binding as Binding;
                    XmlText bindingVal = xDoc.CreateTextNode(binding.Path.Path);
                    bindingNode.AppendChild(bindingVal);
                    dtNode.AppendChild(bindingNode);

                    XmlElement valueNode = xDoc.CreateElement("Value");
                    XmlText valueVal = xDoc.CreateTextNode(dataTrigger.Value.ToString());
                    valueNode.AppendChild(valueVal);
                    dtNode.AppendChild(valueNode);

                    XmlElement styleSettersNode = xDoc.CreateElement("Style");
                    dtNode.AppendChild(styleSettersNode);

                    SaveSetters(xDoc, styleSettersNode, dataTrigger.Setters);
                } else if (trig is Trigger trigger)
                {
                    XmlElement tNode = xDoc.CreateElement("Trigger");
                    parent.AppendChild(tNode);

                    XmlElement bindingNode = xDoc.CreateElement("Property");
                    XmlText bindingVal = xDoc.CreateTextNode(trigger.Property.ToString());
                    bindingNode.AppendChild(bindingVal);
                    tNode.AppendChild(bindingNode);

                    XmlElement valueNode = xDoc.CreateElement("Value");
                    XmlText valueVal = xDoc.CreateTextNode(trigger.Value.ToString());
                    valueNode.AppendChild(valueVal);
                    tNode.AppendChild(valueNode);

                    XmlElement styleSettersNode = xDoc.CreateElement("Style");
                    tNode.AppendChild(styleSettersNode);

                    SaveSetters(xDoc, styleSettersNode, trigger.Setters);
                }
            }
        }

        private static void SaveSetters(XmlDocument xDoc, XmlElement parent, SetterBaseCollection setters)
        {
            foreach (Setter styleSetter in setters)
            {
                XmlElement styleSetterNode = xDoc.CreateElement(styleSetter.Property.ToString());
                XmlText styleSetterVal = xDoc.CreateTextNode(styleSetter.Value.ToString());
                styleSetterNode.AppendChild(styleSetterVal);
                parent.AppendChild(styleSetterNode);
            }
        }
    }
}
