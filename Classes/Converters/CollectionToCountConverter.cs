using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Employees.Classes.Converters
{
    public class CollectionToCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ICollection) value)?.Count ?? 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}