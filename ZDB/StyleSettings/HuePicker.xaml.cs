using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZDB.StyleSettings
{
    /// <summary>
    /// Interaction logic for HuePicker.xaml
    /// </summary>
    public partial class HuePicker : UserControl
    {
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register
            ("Hue", typeof(double), typeof(SaturationBrightnessPicker), new PropertyMetadata(0.0));

        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        public HuePicker()
        {
            InitializeComponent();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Mouse.Capture(null);
            Point pos = e.GetPosition(this);
            Hue = pos.Y / ActualHeight;
        }
    }
}
