using System;
using System.Globalization;

namespace CellCAD.viewmodels
{
    public class DoubleToStringConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString() ?? string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Treat empty/whitespace as "do not update the source"
            if (value is string s && string.IsNullOrWhiteSpace(s))
                return System.Windows.Data.Binding.DoNothing;

            // Try invariant first, then current culture
            if (value is string s1 && double.TryParse(s1, NumberStyles.Float, CultureInfo.InvariantCulture, out var d1))
                return d1;
            if (value is string s2 && double.TryParse(s2, NumberStyles.Float, culture, out var d2))
                return d2;

            // If parsing fails, don’t clobber the ViewModel
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
