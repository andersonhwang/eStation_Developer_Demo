using Serilog;
using System.Windows;

namespace Demo_WPF.Helper
{
    internal class MsgHelper
    {
        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        public static void Infor(string message)
        {
            try
            {
                MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        public static void Error(string message)
        {
            try
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(message);
            }
            catch { }
        }

        /// <summary>
        /// Confirm
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Confirm result</returns>
        public static MessageBoxResult Confirm(string message)
        {
            try
            {
                return MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            catch { return MessageBoxResult.Cancel; }
        }
    }
}
