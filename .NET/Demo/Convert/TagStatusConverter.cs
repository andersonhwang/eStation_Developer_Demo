using Demo_Common.Enum;
using System.Globalization;
using System.Windows.Data;

namespace Demo_WPF.Convert
{
    /// <summary>
    /// AP status converter
    /// </summary>
    [ValueConversion(typeof(TagStatus), typeof(string))]
    public class TagStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) return string.Empty;

            return (TagStatus)value switch
            {
                TagStatus.Init => "Init",
                TagStatus.Sending => "Sending",
                TagStatus.Success => "Success",
                TagStatus.Error => "Error",
                TagStatus.Heartbeat => "Heartbeat",
                _ => "--",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "Init" => TagStatus.Init,
                "Sending" => TagStatus.Sending,
                "Success" => TagStatus.Success,
                "Error" => TagStatus.Error,
                "Heartbeat" => TagStatus.Heartbeat,
                _ => TagStatus.Init,
            };
        }
        #endregion
    }
}
