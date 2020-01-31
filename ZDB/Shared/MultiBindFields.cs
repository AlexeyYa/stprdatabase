using System;
using System.Windows.Data;

namespace ZDB
{
    public class TotalFormatsConverter : IMultiValueConverter
    {
        // values[] = "NumberOfOriginals", "NumberOfCopies", 
        // "Numeration", "Scan", "Threading", "SizeFormat" 
        public object Convert(object[] values, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            int result = (int)values[0] * ((int)values[2] + (int)values[5]) +
                (int)values[1] * (int)values[5] + (int)values[3] + (int)values[4];
            return result.ToString();
        }

        public object[] ConvertBack(object values, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back from sum");
        }
    }
}
