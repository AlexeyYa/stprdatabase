using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                        rowStyle = LoadStyle(xNode.FirstChild);
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
                string ColumnHeader = String.Empty;
                Visibility visibility = Visibility.Visible;
                int DisplayIndex = 0;
                double WidthValue = 40.0;
                DataGridLengthUnitType WidthType = DataGridLengthUnitType.Auto;
                Style ColStyle = new Style();
                foreach (XmlNode childNode in xNode.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "ColumnHeader":
                            ColumnHeader = childNode.InnerText;
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
                        case "Style":
                            ColStyle = LoadStyle(childNode);
                            break;
                    }
                }
                if (ColumnHeader == String.Empty) { break; }
                ColumnInfo column = new ColumnInfo(visibility, DisplayIndex, WidthValue, WidthType, ColumnHeader, ColStyle);
                CInfo.Add(column);
            }

            return CInfo;
        }

        private static Style LoadStyle(XmlNode parent)
        {
            Style style = new Style();
            foreach (XmlNode node in parent)
            {
                switch (node.Name)
                {
                    case "Setters":
                        LoadAllSetters(node, style.Setters);
                        break;
                    case "DataTrigger":
                        LoadDataTrigger(node, style);
                        break;
                    case "Trigger":
                        LoadTrigger(node, style);
                        break;
                }
            }
            return style;
        }

        private static void LoadDataTrigger(XmlNode source, Style style)
        {
            DataTrigger dt = new DataTrigger();
            dt.Binding = new Binding();
            foreach (XmlNode dtNode in source)
            {
                switch (dtNode.Name)
                {
                    case "Binding":
                        dt.Binding = new Binding(dtNode.InnerText);
                        break;
                    case "Value":
                        dt.Value = dtNode.InnerText;
                        break;
                    case "Setters":
                        LoadAllSetters(dtNode, dt.Setters);
                        break;
                }
            }
            style.Triggers.Add(dt);
        }

        private static void LoadTrigger(XmlNode source, Style style)
        {
            Trigger dt = new Trigger();
            foreach (XmlNode dtNode in source)
            {
                switch (dtNode.Name)
                {
                    case "Property":
                        if (dtNode.InnerText == "IsCellSelected")
                        {
                            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromName(
                                dtNode.InnerText, typeof(DataGridAttachedProperties),
                                typeof(DataGridRow));
                            dt.Property = descriptor.DependencyProperty;
                        }
                        else if (dtNode.InnerText == "IsSelected")
                        {
                            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromName(
                                dtNode.InnerText, typeof(DataGridCell),
                                typeof(DataGridCell));
                            dt.Property = descriptor.DependencyProperty;
                        }
                        break;
                    case "Value":
                        dt.Value = (dtNode.InnerText == "True" ? true : false);
                        break;
                    case "Setters":
                        LoadAllSetters(dtNode, dt.Setters);
                        break;
                }
            }
            style.Triggers.Add(dt);
        }

        private static void LoadAllSetters(XmlNode source, SetterBaseCollection styleSetters)
        {
            foreach (XmlNode StyleSetter in source)
            {
                LoadSetter(StyleSetter, styleSetters);
            }
        }

        private static void LoadSetter(XmlNode StyleSetter, SetterBaseCollection styleSetters)
        {
            switch (StyleSetter.Name)
            {
                case "TextAlignment":
                    Enum.TryParse(StyleSetter.InnerText, out TextAlignment alignment);
                    Setter textAlignment = new Setter(TextBlock.TextAlignmentProperty,
                        alignment);
                    styleSetters.Add(textAlignment);
                    break;
                case "FontFamily":
                    Setter fontFamily = new Setter(DataGridCell.FontFamilyProperty,
                        new FontFamily(StyleSetter.InnerText));
                    styleSetters.Add(fontFamily);
                    break;
                case "FontSize":
                    Double.TryParse(StyleSetter.InnerText, out double fontSizeValue);
                    Setter fontSize = new Setter(DataGridCell.FontSizeProperty,
                        fontSizeValue);
                    styleSetters.Add(fontSize);
                    break;
                case "Background":
                    Setter background = new Setter(DataGridCell.BackgroundProperty,
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(StyleSetter.InnerText)));
                    styleSetters.Add(background);
                    break;
                case "Foreground":
                    Setter foreground = new Setter(DataGridCell.ForegroundProperty,
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(StyleSetter.InnerText)));
                    styleSetters.Add(foreground);
                    break;
                case "BorderThickness":
                    Thickness thickness = StringToThickness(StyleSetter.InnerText);
                    Setter borderThickness = new Setter(DataGridRow.BorderThicknessProperty,
                        thickness);
                    styleSetters.Add(borderThickness);
                    //-		Value	{0,1,0,1}	object {System.Windows.Thickness}
//                    Bottom  1   double
//  Left    0   double
//  Right   0   double
//  Top 1   double

                    break;
                case "IsCellSelected":
                    Setter isCellSelected = new Setter(DataGridAttachedProperties.IsCellSelectedProperty,
                        (StyleSetter.InnerText == "True" ? true : false));

                    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromName(
                        "IsCellSelected", typeof(DataGridAttachedProperties),
                        typeof(DataGridRow));
                    
                    isCellSelected.Property = descriptor.DependencyProperty;
                    styleSetters.Add(isCellSelected);
                    break;
            }
        }

        public static Thickness StringToThickness(string s)
        {
            double[] lengths = new double[4];
            int i = 0;

            while (s.Contains(',') || s != String.Empty)
            {
                int index = s.IndexOf(',');
                if (index == -1)
                {
                    lengths[i] = Double.Parse(s);
                    i++;
                    break;
                }
                else
                {
                    lengths[i] = Double.Parse(s.Substring(0, index));
                    s = s.Substring(index + 1);
                    i++;
                }
            }

            switch (i)
            {
                case 1:
                    return new Thickness(lengths[0]);
                case 4:
                    return new Thickness(lengths[0], lengths[1], lengths[2], lengths[3]);
            }
            return new Thickness();
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

                XmlElement colIdNode = xDoc.CreateElement("ColumnHeader");
                XmlText colIdText = xDoc.CreateTextNode(columnInfo.ColumnHeader);
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
                    SaveStyle(xDoc, columnXml, columnInfo.ColStyle);
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
                    styleNode.AppendChild(dtNode);

                    XmlElement bindingNode = xDoc.CreateElement("Binding");
                    Binding binding = dataTrigger.Binding as Binding;
                    XmlText bindingVal = xDoc.CreateTextNode(binding.Path.Path);
                    bindingNode.AppendChild(bindingVal);
                    dtNode.AppendChild(bindingNode);

                    XmlElement valueNode = xDoc.CreateElement("Value");
                    XmlText valueVal = xDoc.CreateTextNode(dataTrigger.Value.ToString());
                    valueNode.AppendChild(valueVal);
                    dtNode.AppendChild(valueNode);

                    SaveSetters(xDoc, dtNode, dataTrigger.Setters);
                } else if (trig is Trigger trigger)
                {
                    XmlElement tNode = xDoc.CreateElement("Trigger");
                    styleNode.AppendChild(tNode);

                    XmlElement propertyNode = xDoc.CreateElement("Property");
                    XmlText propertyVal = xDoc.CreateTextNode(trigger.Property.ToString());
                    propertyNode.AppendChild(propertyVal);
                    tNode.AppendChild(propertyNode);

                    XmlElement valueNode = xDoc.CreateElement("Value");
                    XmlText valueVal = xDoc.CreateTextNode(trigger.Value.ToString());
                    valueNode.AppendChild(valueVal);
                    tNode.AppendChild(valueNode);

                    SaveSetters(xDoc, tNode, trigger.Setters);
                }
            }
        }

        private static void SaveSetters(XmlDocument xDoc, XmlElement parent, SetterBaseCollection setters)
        {
            XmlElement styleNode = xDoc.CreateElement("Setters");
            parent.AppendChild(styleNode);

            foreach (Setter styleSetter in setters)
            {
                XmlElement styleSetterNode = xDoc.CreateElement(styleSetter.Property.ToString());
                XmlText styleSetterVal = xDoc.CreateTextNode(styleSetter.Value.ToString());
                styleSetterNode.AppendChild(styleSetterVal);
                styleNode.AppendChild(styleSetterNode);
            }
        }
    }
}
