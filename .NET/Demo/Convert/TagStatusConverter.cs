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
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
                TagStatus.InvaidKey => "Invalid Key",
                TagStatus.DuplicateToken => "Duplicate Token",
                TagStatus.LcmdIdError => "LCM ID Error",
                TagStatus.LcmdRefreshError => "LCM Refresh Error",
                TagStatus.McuReset => "MCU Reset",
                TagStatus.Unknown => "Unknown",
                _ => "--",
            };
        }

        /// <summary>
        /// Convert back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TagStatus.Unknown; // No convert back
        }
        #endregion
    }
}
