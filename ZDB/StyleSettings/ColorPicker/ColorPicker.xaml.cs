using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dsafa.WpfColorPicker
{
    /// <summary>
    /// User control that contains a color picker.
    /// </summary>
    public partial class ColorPicker : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ColorProperty = 
            DependencyProperty.Register(nameof(Color), typeof(Color), 
            typeof(ColorPicker), new PropertyMetadata(Colors.Red));

        /// <summary>
        /// Creates an instance of the color picker.
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Gets or sets the currently selected color.
        /// </summary>

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Color = (Color)e.NewValue;
        }

        private double _hue;
        private double _saturation = 1;
        private double _brightness = 1;
        private byte _alpha = 255;

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public double Hue
        {
            get => _hue;
            set
            {
                if (value == _hue)
                {
                    return;
                }

                UpdateColorFromHSB();
                _hue = value;
                OnPropertyChanged("Hue");
            }
        }

        public double Saturation
        {
            get => _saturation;
            set
            {
                if (value == _saturation)
                {
                    return;
                }

                UpdateColorFromHSB();
                _saturation = value;
                OnPropertyChanged("Saturation");
            }
        }

        public double Brightness
        {
            get => _brightness;
            set
            {
                if (value == _brightness)
                {
                    return;
                }

                UpdateColorFromHSB();
                _brightness = value;
                OnPropertyChanged("Brightness");
            }
        }

        public byte Alpha
        {
            get => _alpha;
            set
            {
                if (value == _alpha)
                {
                    return;
                }

                UpdateColorFromHSB();
                _alpha = value;
                OnPropertyChanged("Alpha");
            }
        }

        private void UpdateColorFromHSB()
        {   
            var c = ColorHelper.FromHSV(Hue, Saturation, Brightness);
            c.A = Alpha;

            Color = c;
        }
    }
}

