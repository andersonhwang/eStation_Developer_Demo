using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for OTAWindow.xaml
    /// </summary>
    public partial class OTAWindow : Window
    {
        public OTAWindow()
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;
        }
    }
}
