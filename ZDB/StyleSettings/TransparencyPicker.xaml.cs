using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ZDB.StyleSettings
{
    /// <summary>
    /// Interaction logic for TransparencyPicker.xaml
    /// </summary>
    public partial class TransparencyPicker : UserControl
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register
            ("Color", typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0));

        public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register
            ("Alpha", typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0));

        public double Color
        {
            get => (double)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public double Alpha
        {
            get => (double)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value);
        }

        public TransparencyPicker()
        {
            InitializeComponent();
        }
    }

    internal class ColorToSolidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Color)value;
            c.A = 255;
            return c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
