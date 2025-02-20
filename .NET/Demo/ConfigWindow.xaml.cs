using Demo_Common.Entity;
using Demo_WPF.ViewModel;
using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="config">Ap config</param>
        public ConfigWindow(string id, Ap config)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
            if (DataContext is ApConfigViewModel vm) vm.SetConfig(id, config);
        }
    }
}
