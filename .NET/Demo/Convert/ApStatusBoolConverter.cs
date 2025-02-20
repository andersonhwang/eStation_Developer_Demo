using Demo_Common.Enum;
using System.Globalization;
using System.Windows.Data;

namespace Demo_WPF.Convert
{
    /// <summary>
    /// AP status converter
    /// </summary>
    [ValueConversion(typeof(ApStatus), typeof(bool))]
    public class ApStatusBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool)) return false;

            return (ApStatus)value == ApStatus.Online;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => (bool)value ? ApStatus.Online : ApStatus.Offline;
        #endregion
    }
}
