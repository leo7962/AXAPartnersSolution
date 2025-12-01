using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Presentation.Converters
{
    public class StatusMessageToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string message)
            {
                if (message.StartsWith("❌") || message.StartsWith("Error"))
                    return new SolidColorBrush(Color.FromRgb(255, 235, 238)); // Rojo claro
                else if (message.StartsWith("✅"))
                    return new SolidColorBrush(Color.FromRgb(232, 245, 233)); // Verde claro
                else
                    return new SolidColorBrush(Color.FromRgb(225, 245, 254)); // Azul claro
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusMessageToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string message)
            {
                if (message.StartsWith("❌") || message.StartsWith("Error"))
                    return new SolidColorBrush(Color.FromRgb(183, 28, 28)); // Rojo oscuro
                else if (message.StartsWith("✅"))
                    return new SolidColorBrush(Color.FromRgb(27, 94, 32)); // Verde oscuro
                else
                    return new SolidColorBrush(Color.FromRgb(13, 71, 161)); // Azul oscuro
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
