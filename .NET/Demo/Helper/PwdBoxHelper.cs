using System.Windows;
using System.Windows.Controls;

namespace Demo_WPF.Helper
{
    /// <summary>
    /// PasswordBox helper
    /// </summary>
    public static class PwdBoxHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PwdBoxHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PwdBoxHelper), new PropertyMetadata(false, Attach));
        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PwdBoxHelper));
        /// <summary>
        /// Set attach
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        public static void SetAttach(DependencyObject dp, bool value) => dp.SetValue(AttachProperty, value);
        /// <summary>
        /// Get attach
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);
        /// <summary>
        /// Get password
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);
        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);
        /// <summary>
        /// Get IsUpdating
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);
        /// <summary>
        /// Set IsUpdating
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        private static void SetIsUpdating(DependencyObject dp, bool value) => dp.SetValue(IsUpdatingProperty, value);

        /// <summary>
        /// Password change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox) return;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }
        /// <summary>
        /// Attach
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox) return;
            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }
        /// <summary>
        /// Password changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox) return;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
