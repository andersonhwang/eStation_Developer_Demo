using Demo_Common.Helper;
using Demo_Common.Service;
using Demo_WPF.Helper;
using Demo_WPF.Model;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class ApConnectViewModel : ViewModelBase
    {
        private string port = "9071";
        private ConnInfoModel conn = new();

        /// <summary>
        /// Port
        /// </summary>
        public string Port { get { return port; } set { port = value; NotifyPropertyChanged(nameof(Port)); } }
        /// <summary>
        /// AP information
        /// </summary>
        public ConnInfoModel Conn { get { return conn; } set { conn = value; NotifyPropertyChanged(nameof(conn)); } }
        /// <summary>
        /// Command - Check
        /// </summary>
        public ICommand CmdCheck { get; set; }
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
        public ApConnectViewModel()
        {
            Conn = FileHelper.TryGet<ConnInfoModel>(nameof(ConnInfoModel) + ".json");

            CmdCheck = new MyCommand(DoCheck, CanCheck);
            CmdRun = new MyCommand(DoRun, CanRun);
            CmdCertificate = new MyCommand(DoCertificate, CanCertificate);
        }

        /// <summary>
        /// Check check
        /// </summary>
        /// <returns></returns>
        private bool CanCheck(object obj) => !string.IsNullOrEmpty(Port);

        /// <summary>
        /// Do check
        /// </summary>
        /// <param name="obj"></param>
        private void DoCheck(object obj)
        {
            if (Check() > 0)
            {
                MsgHelper.Infor($"Port {Conn.Port} is avaliable");
            }
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
            var result = Check();
            if (result > 0)
            {
                Conn.Port = result;
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
            var dialog = new CertificateWindow((path, key) => { Conn.Certificate = path; Conn.CertificateKeyM = key; });
            dialog.ShowDialog();
        }

        /// <summary>
        /// Check connection information
        /// </summary>
        /// <returns>Check result</returns>
        private int Check()
        {
            if (string.IsNullOrEmpty(Port))
            {
                MsgHelper.Error("Port is mandatory");
                return -1;
            }

            if (!int.TryParse(Port, out int value))
            {
                MsgHelper.Error($"Invliad port {Port}");
                return 0;
            }

            if (value < 1000 || value > 0xFFFF)
            {
                MsgHelper.Error($"Invliad port {value}");
                return 0;
            }

            var listener = new TcpListener(IPAddress.Any, value);
            try
            {
                listener.Start();
                listener.Stop();
                return value;
            }
            catch (SocketException)
            {
                MsgHelper.Error($"Port {value} is unavaliable");
                return -2;
            }
        }
    }
}
