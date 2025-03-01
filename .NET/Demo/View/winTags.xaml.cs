using Demo_Common.Helper;
using Demo_WPF.Helper;
using System.IO;
using System.Text;
using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for TagsWindow.xaml
    /// </summary>
    public partial class winTags : Window
    {
        private readonly List<string> _tagIDList = [];

        public delegate void UpdateTagIDList(List<string> tagIDList);
        public event UpdateTagIDList UpdateTagIDListHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="list">Tags ID list</param>
        public winTags(List<string> list)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            if (list != null) _tagIDList.AddRange(list);
        }

        /// <summary>
        /// Window load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder builder = new();
                foreach (var id in _tagIDList) builder.AppendLine(id);
                txtIDList.Text = builder.ToString();
            }
            catch (Exception ex)
            {
                MsgHelper.Error(ex.Message);
            }
        }

        /// <summary>
        /// Button save click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hash = new HashSet<string>();
                var items = txtIDList.Text.Trim().ToUpper().Split('\r');
                var builder = new StringBuilder();
                foreach (var item in items)
                {
                    var id = item.Trim('\n');
                    if (!TagHelper.RegTagID.IsMatch(id) || hash.Contains(id)) continue;
                    hash.Add(id);
                    builder.AppendLine(id);
                }
                File.WriteAllText("TagList.txt", builder.ToString());
                Close();
            }
            catch (Exception ex)
            {
                MsgHelper.Error(ex.Message);
            }
        }

        /// <summary>
        /// Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
