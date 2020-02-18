using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.IO;
using System.Windows.Markup;
using System.Text;
using System.Xml;

namespace ZDB.StyleSettings
{
    class StyleSelectorMVVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private double fontSize;
        public double FontSize
        {
            get => fontSize;
            set
            {
                if (fontSize != value)
                {
                    fontSize = value;
                    Setter fontSizeSetter = new Setter(DataGridCell.FontSizeProperty,
                        value);
                    foreach (var column in GetSelectedColumns())
                    {
                        column.CellStyle = ChangeStyle(column.CellStyle, fontSizeSetter);
                    }
                    OnPropertyChanged("FontSize");
                }
            }
        }
        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get => fontFamily;
            set
            {
                if (fontFamily != value)
                {
                    fontFamily = value;
                    Setter fontFamilySetter = new Setter(DataGridCell.FontFamilyProperty,
                        value);
                    foreach (var column in GetSelectedColumns())
                    {
                        column.CellStyle = ChangeStyle(column.CellStyle, fontFamilySetter);
                    }
                    OnPropertyChanged("FontFamily");
                }
            }
        }
        private Color foregroundColor = Color.FromRgb(255, 0, 0);
        public Color ForegroundColor
        {
            get => foregroundColor;
            set
            {
                if (foregroundColor != value)
                {
                    foregroundColor = value;
                    Setter foregroundSetter = new Setter(DataGridCell.ForegroundProperty,
                        new SolidColorBrush(value));
                    foreach (var column in GetSelectedColumns())
                    {
                        column.CellStyle = ChangeStyle(column.CellStyle, foregroundSetter);
                    }
                    OnPropertyChanged("ForegroundColor");
                }
            }
        }
        private Color backgroundColor = Color.FromRgb(255,0,0);
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    Setter backgroundSetter = new Setter(DataGridCell.BackgroundProperty,
                        new SolidColorBrush(value));
                    foreach (var column in GetSelectedColumns())
                    {
                        column.CellStyle = ChangeStyle(column.CellStyle, backgroundSetter);
                    }
                    OnPropertyChanged("BackgroundColor");
                }
            }
        }
        private string horizontalAlignment;
        public string HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                if (horizontalAlignment != value)
                {
                    horizontalAlignment = value;

                    Setter horizontalAlignmentSetter = new Setter(TextBlock.TextAlignmentProperty,
                        TextAlignment.Justify);
                    switch (value)
                    {
                        case "Left":
                            horizontalAlignmentSetter = new Setter(TextBlock.TextAlignmentProperty,
                                TextAlignment.Left);
                            break;
                        case "Center":
                            horizontalAlignmentSetter = new Setter(TextBlock.TextAlignmentProperty,
                                TextAlignment.Center);
                            break;
                        case "Right":
                            horizontalAlignmentSetter = new Setter(TextBlock.TextAlignmentProperty,
                                TextAlignment.Right);
                            break;
                        case "Justify":
                            horizontalAlignmentSetter = new Setter(TextBlock.TextAlignmentProperty,
                                TextAlignment.Justify);
                            break;
                    }
                    foreach (var column in GetSelectedColumns())
                    {
                        column.CellStyle = ChangeStyle(column.CellStyle, horizontalAlignmentSetter);
                    }
                    OnPropertyChanged("HorizontalAlignment");
                }
            }
        }

        private string verticalAlignment;
        public string VerticalAlignmentField
        {
            get => verticalAlignment;
            set
            {
                if (verticalAlignment != value)
                {
                    verticalAlignment = value;
                    // Copy in GridSettingsManager
                    string tmpXaml = "<Setter Property=\"Control.Template\""+
                        " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Setter.Value>" +
                        "<ControlTemplate TargetType=\"DataGridCell\">"+
                        "<Grid Background=\"{TemplateBinding Panel.Background}\">"+
                        "<ContentPresenter Content=\"{TemplateBinding ContentControl.Content}\""+
                        " ContentTemplate=\"{TemplateBinding ContentControl.ContentTemplate}\""+
                        " ContentStringFormat=\"{TemplateBinding ContentControl.ContentStringFormat}\""+
                        " VerticalAlignment=\"" + value + "\" />"+
                        "</Grid></ControlTemplate></Setter.Value></Setter>";

                    StringReader stringReader = new StringReader(tmpXaml);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    Setter verticalAlignmentSetter = (Setter)XamlReader.Load(xmlReader);
                    foreach (var column in GetSelectedColumns())
                    {
                        column.CellStyle = ChangeStyle(column.CellStyle, verticalAlignmentSetter);
                    }
                    OnPropertyChanged("VerticalAlignment");
                }
            }
        }

        private ThicknessConverter thicknessConverter = new ThicknessConverter();
        private Thickness margin;
        public string Margin
        {
            get => thicknessConverter.ConvertToString(margin);
            set
            {
                try
                {
                    var thickness = (Thickness)thicknessConverter.ConvertFromString(value);
                    if (margin != thickness)
                    {
                        margin = thickness;



                        Setter thicknessSetter = new Setter(Border.PaddingProperty,
                            thickness);



                        foreach (var column in GetSelectedColumns())
                        {
                            column.CellStyle = ChangeStyle(column.CellStyle, thicknessSetter);
                        }
                        OnPropertyChanged("Margin");
                    }
                }
                catch { }
            }
        }


        public IList<DataGridCellInfo> SelectedCells;

        public StyleSelectorMVVM(IList<DataGridCellInfo> selectedCells)
        {
            SelectedCells = selectedCells;
        }

        private List<DataGridColumn> GetSelectedColumns()
        {
            List<DataGridColumn> result = new List<DataGridColumn>();
            foreach (var cell in SelectedCells)
            {
                if (!result.Contains(cell.Column))
                {
                    result.Add(cell.Column);
                }
            }
            return result;
        }

        private Style CopyOldStyle(Style oldStyle)
        {
            string styleXaml = XamlWriter.Save(oldStyle);
            StringReader stringReader = new StringReader(styleXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Style style = (Style)XamlReader.Load(xmlReader);

            return style;
        }

        private void RemoveSameSetter(Style style, Setter setter)
        {
            var oldSetter = style.Setters.FirstOrDefault(
                        s => ((Setter)s).Property == setter.Property);
            if (oldSetter != null)
                style.Setters.Remove(oldSetter);
        }

        private Style ChangeStyle(Style oldStyle, Setter setter)
        {
            Style style = CopyOldStyle(oldStyle);
            RemoveSameSetter(style, setter);
            style.Setters.Add(setter);
            return style;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
