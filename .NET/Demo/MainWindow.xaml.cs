using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ucDebugRequest.SetDebugType(true);
            ucDebugResponse.SetDebugType(false);
        }
    }
}