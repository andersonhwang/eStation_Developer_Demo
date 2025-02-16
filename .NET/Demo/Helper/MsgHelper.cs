using System.Windows;

namespace Demo_WPF.Helper
{
    internal class MsgHelper
    {
        public static void Infor(string message)
        {
            try
            {
                MessageBox.Show(message, "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

        public static void Error(string message)
        {
            try
            {
                MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        }

        public static MessageBoxResult Confirm(string message)
        {
            try
            {
                return MessageBox.Show(message, "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            catch { return MessageBoxResult.Cancel; }
        }
    }
}
