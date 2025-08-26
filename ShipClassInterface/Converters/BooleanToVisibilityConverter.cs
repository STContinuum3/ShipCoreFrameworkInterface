using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShipClassInterface.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool invert = parameter?.ToString()?.Equals("Inverted", StringComparison.OrdinalIgnoreCase) == true;
                
                if (invert)
                    boolValue = !boolValue;
                    
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool invert = parameter?.ToString()?.Equals("Inverted", StringComparison.OrdinalIgnoreCase) == true;
                bool result = visibility == Visibility.Visible;
                
                return invert ? !result : result;
            }
            
            return false;
        }
    }
}