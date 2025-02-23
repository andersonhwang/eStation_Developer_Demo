using Demo_Common.Entity;
using Demo_WPF.ViewModel;
using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class winConfig : Window
    {
        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="infor">Ap config</param>
        public winConfig(string id, ApInfor infor)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
            if (DataContext is ApConfigViewModel vm) vm.SetConfig(id, infor);
        }
    }
}
