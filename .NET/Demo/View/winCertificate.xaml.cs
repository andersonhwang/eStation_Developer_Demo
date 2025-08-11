using System.Diagnostics;
using System.IO;
using System.Windows;
using Demo_Common.Helper;
using Demo_WPF.Helper;
using Microsoft.Win32;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for CertificateWindow.xaml
    /// </summary>
    public partial class winCertificate : Window
    {
        private readonly Action<string, string> ActSave;

        public winCertificate(Action<string, string> action)
        {
            InitializeComponent();

            ActSave = action;
            Owner = Application.Current.MainWindow;
        }

        /// <summary>
        /// Button cancel click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Button save click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var path = txtPath.Text.Trim();
            var key = txtKey.Text;
            if (path.Length == 0)
            {
                MsgHelper.Error("Certificate_File_Is_Required");
                return;
            }
            if (!File.Exists(path))
            {
                MsgHelper.Error("File_Not_Exist:" + path);
                return;
            }
            if (string.IsNullOrEmpty(key))
            {
                MsgHelper.Error("Key_Is_Required");
                return;
            }

            var check = FileHelper.GetCertificate(path, key);
            if (check.Length == 0)
            {
                MsgHelper.Error("Invalid_Certificate_Or_Key");
                return;
            }
            ActSave(path, key);
            Close();
        }

        /// <summary>
        /// Button browse click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                DefaultDirectory = Path.GetDirectoryName(Environment.ProcessPath),
                FileName = "server",
                DefaultExt = ".pfx",
                Filter = "Personal Information Exchange (.pfx)|*.pfx"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                txtPath.Text = dialog.FileName;
            }
        }
    }
}
