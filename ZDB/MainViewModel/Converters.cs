using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ZDB.Database;

namespace ZDB.MainViewModel
{
    /// <summary>
    /// Converter for summing int values in groups
    /// </summary>
    public class SumValues : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cvg = value as CollectionViewGroup;
            var field = parameter as string;
            if (cvg == null || field == null) return null;

            // Sum of Entries
            if (cvg.Items[0] is Entry)
            {
                return cvg.Items.Sum(r => (int)(r as Entry)[field]);
            } 
            // Sum of Groups
            else if (cvg.Items[0] is CollectionViewGroup)
            {
                return cvg.Items.Sum(r => (int)(Convert(r, targetType, parameter, culture)));
            }
            // Neither, shouldn't ever get there!!!
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Grouping months
    public class DateToMonthsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;

            if (!date.HasValue) return null;

            return new DateTime(date.Value.Year, date.Value.Month, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Grouping quarters
    public class DateToQuartersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;

            if (!date.HasValue) return null;

            // return date day 2 for grouping quarters
            return new DateTime(date.Value.Year, date.Value.Month - (date.Value.Month - 1) % 3, 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for display
    public class DateToTextMonthsConverter : IValueConverter
    {
        static IList<string> monthsNames = new List<string>
        { "NULL", "Январь", "Февраль" , "Март" , "Апрель" , "Май" , "Июнь" , "Июль" , "Август"
        , "Сентябрь" , "Октябрь" , "Ноябрь", "Декабрь"  };
        static IList<string> quartersNames = new List<string>
        { "Первый квартал" , "Второй квартал" , "Третий квартал", "Четвертый квартал"  };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;

            if (!date.HasValue) return null;
            // Day 1 for months groups
            if (date.Value.Day == 1)
            {
                return monthsNames[date.Value.Month] + " " + date.Value.Year;
            } 
            // Day 2 for quarters groups
            else if(date.Value.Day == 2)
            {
                return quartersNames[date.Value.Month / 3] + " " + date.Value.Year;
            }
            // Don't ever get close to me or my son
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
