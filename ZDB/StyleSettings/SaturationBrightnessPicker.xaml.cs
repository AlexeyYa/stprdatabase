using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZDB.StyleSettings
{
    /// <summary>
    /// Interaction logic for SaturationBrightnessPicker.xaml
    /// </summary>
    public partial class SaturationBrightnessPicker : UserControl
    {
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register
            ("Hue", typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0));
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register
            ("Saturation", typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0));
        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register
            ("Brightness", typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0));

        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        public double Saturation
        {
            get => (double)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        public double Brightness
        {
            get => (double)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }

        public SaturationBrightnessPicker()
        {
            InitializeComponent();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Mouse.Capture(null);
            Point pos = e.GetPosition(this);
            Saturation = pos.X / ActualWidth;
            Brightness = 1 - (pos.Y / ActualHeight);
        }
    }
}
