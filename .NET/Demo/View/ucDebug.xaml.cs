using Demo_WPF.Model;
using Demo_WPF.ViewModel;
using System.Windows.Controls;

namespace Demo_WPF.View
{
    /// <summary>
    /// Interaction logic for ucDebug.xaml
    /// </summary>
    public partial class ucDebug : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ucDebug()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set debug type
        /// </summary>
        /// <param name="request">Request</param>
        public void SetDebugType(bool request)
        {
            if (DataContext is DebugViewModel vm)
            {
                var info = request
                    ? new DebugInfo { Request = request, Header = "Debug->Request", Description = "※ From Server Side" }
                    : new DebugInfo { Request = request, Header = "Debug->Response", Description = "※ From AP Side" };
                vm.SetInfo(info);
            }
        }
    }
}
