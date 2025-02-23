using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for OTAWindow.xaml
    /// </summary>
    public partial class winOTA : Window
    {
        public winOTA()
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
        }
    }
}
