using Demo_Common.Helper;
using Demo_Common.Service;
using Demo_WPF.Model;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class ApConnectViewModel : TcpViewModelBase
    {
        private ConnInfoModel conn = new();
        /// <summary>
        /// AP information
        /// </summary>
        public ConnInfoModel Conn { get { return conn; } set { conn = value; NotifyPropertyChanged(nameof(conn)); } }
        /// <summary>
        /// Command - Run
        /// </summary>
        public ICommand CmdRun { get; set; }
        /// <summary>
        /// Command - Certificate
        /// </summary>
        public ICommand CmdCertificate { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApConnectViewModel() : base()
        {
            Conn = FileHelper.TryGet<ConnInfoModel>(nameof(ConnInfoModel) + ".json");

            CmdRun = new MyCommand(DoRun, CanRun);
            CmdCertificate = new MyCommand(DoCertificate, CanCertificate);
        }

        /// <summary>
        /// Can connect
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Result</returns>
        private bool CanRun(object arg)
        {
            return true;
        }

        /// <summary>
        /// Do conenct
        /// </summary>
        /// <param name="obj"></param>
        private void DoRun(object obj)
        {
            var ap = CheckPort();
            if (ap > 0)
            {
                Conn.Port = ap;
                IsRun = SendService.Instance.Run(Conn);
            }
            if (IsRun)
            {
                FileHelper.Save(Conn);
            }
        }

        /// <summary>
        /// Can certificate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanCertificate(object obj) => Conn.EncryptM;

        /// <summary>
        /// Do certificate
        /// </summary>
        /// <param name="obj"></param>
        private void DoCertificate(object obj)
        {
            var dialog = new winCertificate((path, key) => { Conn.Certificate = path; Conn.CertificateKeyM = key; });
            dialog.ShowDialog();
        }
    }
}