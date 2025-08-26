using System;
using System.Globalization;
using System.Windows.Data;

namespace ShipClassInterface.Converters
{
    public class BoolToGridTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled && isEnabled && parameter is string gridType)
            {
                return gridType + " ";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}