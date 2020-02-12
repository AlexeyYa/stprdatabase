using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using ZDB.MainViewModel;

namespace ZDB.StyleSettings
{

    // Loading data structure
    public struct ViewSettings
    {
        public ViewSettings(DGEStyle DataGridStyle, CVSStyle CollectionViewSourceStyle)
        {
            dataGridStyle = DataGridStyle;
            collectionViewSourceStyle = CollectionViewSourceStyle;
        }
        public DGEStyle dataGridStyle;
        public CVSStyle collectionViewSourceStyle;
    }

    public static class GridViewSettingsManager
    {
        public static ViewSettings LoadFromXML(string path)
        {
            // DGEStyle variables
            List<ColumnInfo> CInfo = new List<ColumnInfo>();
            Style rowStyle = new Style();
            int frozenColumnCount = 0;

            // CVSStyle variables
            List<Grouping> groupingList = new List<Grouping>();

            // Filters variables

            // XmlDocument reader
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
                    case "FrozenColumnCount":
                        Int32.TryParse(xNode.InnerText, out frozenColumnCount);
                        break;
                    case "DataGroupings":
                        groupingList = LoadGroupings(xNode);
                        break;
                    case "Filters":
                        break;
                }
            }
            return new ViewSettings(
                new DGEStyle(CInfo, rowStyle, frozenColumnCount),
                new CVSStyle(groupingList));
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
                string StringFormat = null;
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
                        case "StringFormat":
                            StringFormat = childNode.InnerText;
                            break;
                    }
                }
                if (ColumnHeader == String.Empty) { break; }
                ColumnInfo column = new ColumnInfo(visibility, DisplayIndex, WidthValue, WidthType, 
                                                   ColumnHeader, ColStyle, StringFormat);
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
                case "Template":

                    StringReader stringReader = new StringReader(StyleSetter.InnerText);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    Setter templateSetter = (Setter)XamlReader.Load(xmlReader);

                    styleSetters.Add(templateSetter);
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

        private static List<Grouping> LoadGroupings(XmlNode groupingsListNode)
        {
            List<Grouping> result = new List<Grouping>();
            
            foreach (XmlNode groupingNode in groupingsListNode)
            {
                string prop = String.Empty;
                string conv = String.Empty;
                foreach (XmlNode childNode in groupingNode)
                {
                    switch (childNode.Name)
                    {
                        case "Property":
                            prop = childNode.InnerText;
                            break;
                        case "Converter":
                            conv = childNode.InnerText;
                            break;
                    }
                }
                result.Add(new Grouping(prop, conv));
            }

            return result;
        }

        // Saving to xml
        public static void SaveToXML(ViewSettings viewSettings, string path)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement xRoot = xDoc.DocumentElement;
            xDoc.InsertBefore(xmlDeclaration, xRoot);
            
            // DataGrid settings
            XmlElement mainElement = xDoc.CreateElement("DGSettings");
            xDoc.AppendChild(mainElement);
            // Columns
            SaveColumns(xDoc, mainElement, viewSettings.dataGridStyle.CInfo);
            // RowStyle
            XmlElement rowStyleNode = xDoc.CreateElement("RowStyle");
            mainElement.AppendChild(rowStyleNode);
            SaveStyle(xDoc, rowStyleNode, viewSettings.dataGridStyle.rowStyle);
            // FrozenColumns to fix first columns in place
            XmlElement frozenColumnCountNode = xDoc.CreateElement("FrozenColumnCount");
            mainElement.AppendChild(frozenColumnCountNode);
            XmlText frozenColumnCountText = xDoc.CreateTextNode(viewSettings.dataGridStyle.frozenColumnCount.ToString());
            frozenColumnCountNode.AppendChild(frozenColumnCountText);

            // CollectionViewSource settings
            SaveCVSStyle(xDoc, mainElement, viewSettings.collectionViewSourceStyle);

            // Filters settings

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

                if (columnInfo.StringFormat != null)
                {
                    XmlElement stringFormatNode = xDoc.CreateElement("StringFormat");
                    XmlText stringFormatVal = xDoc.CreateTextNode(columnInfo.StringFormat.ToString());
                    stringFormatNode.AppendChild(stringFormatVal);
                    columnXml.AppendChild(stringFormatNode);
                }

                if (columnInfo.ColStyle != null)
                {
                    SaveStyle(xDoc, columnXml, columnInfo.ColStyle);
                }
            }
        }

        private static void SaveStyle(XmlDocument xDoc, XmlElement parent, Style style)
        {

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
                }
                else if (trig is Trigger trigger)
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
                if (styleSetter.Property.ToString() == "Template")
                {
                    XmlElement styleSetterNode = xDoc.CreateElement(styleSetter.Property.ToString());

                    string setterXaml = XamlWriter.Save(styleSetter);
                    XmlText styleSetterVal = xDoc.CreateTextNode(setterXaml);
                    styleSetterNode.AppendChild(styleSetterVal);
                    styleNode.AppendChild(styleSetterNode);
                }
                else
                {
                    XmlElement styleSetterNode = xDoc.CreateElement(styleSetter.Property.ToString());
                    XmlText styleSetterVal = xDoc.CreateTextNode(styleSetter.Value.ToString());
                    styleSetterNode.AppendChild(styleSetterVal);
                    styleNode.AppendChild(styleSetterNode);
                }
            }
        }

        private static void SaveCVSStyle(XmlDocument xDoc, XmlElement parent, CVSStyle cvsStyle)
        {
            XmlElement groupingListNode = xDoc.CreateElement("DataGroupings");
            parent.AppendChild(groupingListNode);

            foreach (var groupingPair in cvsStyle.groups)
            {
                XmlElement groupingNode = xDoc.CreateElement("Grouping");
                groupingListNode.AppendChild(groupingNode);

                XmlElement groupingPropertyNode = xDoc.CreateElement("Property");
                XmlText groupingPropertyValue = xDoc.CreateTextNode(groupingPair.field.ToString());
                groupingPropertyNode.AppendChild(groupingPropertyValue);
                groupingNode.AppendChild(groupingPropertyNode);

                XmlElement groupingConverterNode = xDoc.CreateElement("Converter");
                XmlText groupingConverterValue = xDoc.CreateTextNode(groupingPair.converter.ToString());
                groupingConverterNode.AppendChild(groupingConverterValue);
                groupingNode.AppendChild(groupingConverterNode);
            }
        }
    }
}
