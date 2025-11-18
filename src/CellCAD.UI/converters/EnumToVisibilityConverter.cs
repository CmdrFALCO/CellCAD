using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CellCAD.converters
{
    /// <summary>
    /// Converts an enum value to Visibility based on matching a parameter.
    /// Returns Visible if the enum matches the parameter, Collapsed otherwise.
    /// </summary>
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            return value.ToString() == parameter.ToString()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
