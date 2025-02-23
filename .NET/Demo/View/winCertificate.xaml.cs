using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for CertificateWindow.xaml
    /// </summary>
    public partial class winCertificate : Window
    {
        public winCertificate(Action<string, string> action)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
        }
    }
}
