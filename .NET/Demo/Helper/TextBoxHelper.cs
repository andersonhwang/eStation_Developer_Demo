using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Demo_WPF.Helper
{
    /// <summary>
    /// Textbox helper
    /// </summary>
    internal class TextBoxHelper
    {
        private static readonly Regex Regex = new("[^0-9]+"); //regex that matches disallowed text

        /// <summary>
        /// Number only
        /// </summary>
        public static readonly DependencyProperty NumberOnlyProperty = DependencyProperty.RegisterAttached(
            "NumberOnly", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(OnNumberOnlyChanged));
        public static bool GetNumberOnly(DependencyObject obj) => (bool)obj.GetValue(NumberOnlyProperty);
        public static void SetNumberOnly(DependencyObject obj, bool value) => obj.SetValue(NumberOnlyProperty, value);

        private static void OnNumberOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox txt)
            {
                txt.SetValue(InputMethod.IsInputMethodEnabledProperty, !(bool)e.NewValue);
                txt.PreviewTextInput -= TxtInput;
                if(!(bool)e.NewValue) txt.PreviewTextInput += TxtInput;
            }            
        }

        /// <summary>
        /// Text input handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TxtInput(object sender, TextCompositionEventArgs e)
        {
                e.Handled = Regex.IsMatch(e.Text);
        }
    }
}
