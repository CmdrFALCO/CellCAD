using System;
using System.Globalization;
using System.Windows.Data;

namespace CellCAD.converters
{
    /// <summary>
    /// Converts null values to "—" (em dash) for display
    /// </summary>
    public class NullToDashConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "—";

            // Handle nullable types
            if (value is double d)
                return d.ToString("F2", culture);

            return value.ToString() ?? "—";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Read-only binding, no conversion back needed
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
