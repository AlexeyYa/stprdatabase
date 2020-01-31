using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

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
                    OnPropertyChanged("FontFamily");
                }
            }
        }
        private Color foregroundColor;
        public Color ForegroundColor
        {
            get => foregroundColor;
            set
            {
                if (foregroundColor != value)
                {
                    foregroundColor = value;
                    OnPropertyChanged("ForegroundColor");
                }
            }
        }
        private Color backgroundColor;
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    OnPropertyChanged("BackgroundColor");
                }
            }
        }
        private TextAlignment horizontalAlignment;
        public TextAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                if (horizontalAlignment != value)
                {
                    horizontalAlignment = value;
                    OnPropertyChanged("HorizontalAlignment");
                }
            }
        }



        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
