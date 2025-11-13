using System;
using System.Globalization;
using System.Windows.Data;

namespace CellCAD.converters
{
    /// <summary>
    /// Converts an enum value to a boolean for RadioButton binding
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return System.Windows.Data.Binding.DoNothing;

            if ((bool)value)
            {
                return Enum.Parse(targetType, parameter.ToString()!);
            }

            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
