using Demo_Common.Enum;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Demo_WPF.Convert
{
    /// <summary>
    /// AP status converter
    /// </summary>
    [ValueConversion(typeof(Pattern), typeof(bool))]
    public class PatternConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool)) return false;

            if ((Pattern)value == Pattern.Key) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Pattern)) return Pattern.UpdateDisplay;

            return (bool)value ? Pattern.Key : Pattern.UpdateDisplay;
        }
    }

    /// <summary>
    /// AP status converter
    /// </summary>
    [ValueConversion(typeof(Pattern), typeof(Visibility))]
    public class KeyVisiableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility)) return Visibility.Collapsed;

            if ((Pattern)value == Pattern.Key) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Pattern)) return Pattern.UpdateDisplay;

            return (Visibility)value == Visibility.Visible ? Pattern.Key : Pattern.UpdateDisplay;
        }
    }
}
