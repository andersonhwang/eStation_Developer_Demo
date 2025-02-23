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
        private string webPort = "9070";
        private string apPort = "9071";
        private ConnInfoModel conn = new();

        /// <summary>
        /// Web port
        /// </summary>
        public string WebPort { get { return webPort; } set { webPort = value; NotifyPropertyChanged(nameof(WebPort)); } }
        /// <summary>
        /// AP port
        /// </summary>
        public string ApPort { get { return apPort; } set { apPort = value; NotifyPropertyChanged(nameof(ApPort)); } }
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
        private bool CanCheck(object obj) => obj.ToString() == "W" ? !string.IsNullOrEmpty(WebPort) : !string.IsNullOrEmpty(ApPort);

        /// <summary>
        /// Do check
        /// </summary>
        /// <param name="obj"></param>
        private void DoCheck(object obj)
        {
            var check = obj.ToString() switch
            {
                "W" => CheckPort(WebPort, "Web"),
                "A" => CheckPort(ApPort, "AP"),
                _ => 0,
            };

            if (check > 0)
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
            var web = CheckPort(WebPort, "Web");
            if (web > 0)
            {
                WebService.Instance.Run(web);
            }

            var ap = CheckPort(ApPort, "AP");
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

        /// <summary>
        /// Check port
        /// </summary>
        /// <param name="port">Port number</param>
        /// <param name="type">Port type</param>
        /// <returns>Check result</returns>
        private int CheckPort(string port, string type)
        {
            if (string.IsNullOrEmpty(port))
            {
                MsgHelper.Error($"{type} port is mandatory");
                return -1;
            }

            if (!int.TryParse(port, out int value))
            {
                MsgHelper.Error($"Invliad {type} port: {port}");
                return 0;
            }

            if (value < 1000 || value > 0xFFFF)
            {
                MsgHelper.Error($"Invliad {type} port: {value}");
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
                MsgHelper.Error($"{type} port {value} is unavaliable");
                return -2;
            }
            finally
            {
                listener.Dispose();
            }
        }
    }
}