using System.Globalization;
using System.Windows.Data;

namespace Demo_WPF.Convert
{
    /// <summary>
    /// AP status converter
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class BatteryConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool)) return string.Empty;

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
        #endregion
    }
}
