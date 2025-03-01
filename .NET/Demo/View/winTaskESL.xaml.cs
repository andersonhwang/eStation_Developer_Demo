using Demo_WPF.ViewModel;
using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class winTaskESL : Window
    {
        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="tags">Tags ID list</param>
        /// <param name="action">Action - send handler</param>
        public winTaskESL(int token, string[] tags, Action<string[]> action)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            ((TaskEslViewModel)DataContext).SetTags(token, tags, action);
        }
    }
}
