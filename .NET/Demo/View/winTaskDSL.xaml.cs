using Demo_WPF.ViewModel;
using System.Windows;

namespace Demo_WPF.View
{
    /// <summary>
    /// Interaction logic for winTaskDSL.xaml
    /// </summary>
    public partial class winTaskDSL : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="tags">Tags ID list</param>
        /// <param name="action">Action - send handler</param>
        public winTaskDSL(int token, string[] tags, Action<string[]> action)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            ((TaskDslViewModel)DataContext).SetTags(token, tags, action);
        }
    }
}
