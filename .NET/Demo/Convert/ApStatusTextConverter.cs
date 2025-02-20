using Demo_Common.Enum;
using System.Globalization;
using System.Windows.Data;

namespace Demo_WPF.Convert
{
    /// <summary>
    /// AP status converter
    /// </summary>
    [ValueConversion(typeof(ApStatus), typeof(string))]
    public class ApStatusTextConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) return string.Empty;

            return (ApStatus)value switch
            {
                ApStatus.Connecting => "Connecting",
                ApStatus.Online => "Online",
                ApStatus.Offline => "Offline",
                _ => "--",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "Connecting" => ApStatus.Connecting,
                "Online" => ApStatus.Online,
                "Offline" => ApStatus.Offline,
                _ => ApStatus.Init,
            };
        }
        #endregion
    }
}
