using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Employees.Classes.Converters
{
    public class WindowModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var windowMode = (WindowMode) value;
            if (parameter == null) throw new InvalidOperationException("The parameter must be in this converter");
            var parameterWindowMode = (WindowMode) parameter;
            return windowMode == parameterWindowMode ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}