using System;
using System.Globalization;
using System.Windows.Data;

namespace Employees.Classes.Converters
{
    public class EmployeeToPassportSeriesNumber : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var passportNumberSeries = (string) value;
            return passportNumberSeries != null ? 
                passportNumberSeries.FormatPassportSeriesNumber() :
                string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}