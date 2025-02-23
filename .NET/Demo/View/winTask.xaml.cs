using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class winTask : Window
    {
        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="tags"></param>
        public winTask(List<string> tags)
        {
            InitializeComponent();
        }
    }
}
