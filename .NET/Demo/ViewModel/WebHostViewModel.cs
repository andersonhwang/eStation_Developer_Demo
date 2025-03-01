using Demo_Common.Entity;
using Demo_Common.Service;
using Demo_WPF.Helper;
using Demo_WPF.Model;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class WebHostViewModel : TcpViewModelBase
    {
        private bool isRun = false;
        private WebInfo info = new();
        private ObservableCollection<NetworkInfo> networks = [];
        /// <summary>
        /// Is run
        /// </summary>
        public bool IsRun { get => isRun; set { isRun = value; NotifyPropertyChanged(nameof(IsRun)); } }
        /// <summary>
        /// Web Infor
        /// </summary>
        public WebInfo Info { get => info; set { info = value; NotifyPropertyChanged(nameof(Info)); } }
        /// <summary>
        /// Networks
        /// </summary>
        public ObservableCollection<NetworkInfo> Networks { get => networks; set { networks = value; NotifyPropertyChanged(nameof(Networks)); } }
        /// <summary>
        /// Command - OTA
        /// </summary>
        public ICommand CmdOTA { get; set; }
        /// <summary>
        /// Command - Run
        /// </summary>
        public ICommand CmdRun { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public WebHostViewModel() : base()
        {
            Port = "9070";  // Web host port

            CmdOTA = new MyAsyncCommand(DoOTA, CanOTA);
            CmdRun = new MyCommand(DoRun, CanRun);

            // Network adapter
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length == 0) return;
            foreach (NetworkInterface ni in nics)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Networks.Add(new NetworkInfo { Adapter = ni.Name, IP = ip.Address.ToString() });
                    }
                }
            }

            SendService.Instance.Register(SelectTags);
        }

        /// <summary>
        /// Run web host    
        /// </summary>
        /// <param name="obj"></param>
        private void DoRun(object obj)
        {
            try
            {
                var port = CheckPort();
                if (port > 0)
                {
                    WebService.Instance.Run(Info.IP, port);
                    IsRun = WebService.Instance.IsRun;
                }
            }
            catch (Exception ex)
            {
                MsgHelper.Error("Run_Web_Host_Err:" + ex.Message);
            }
        }

        /// <summary>
        /// Do OTA
        /// </summary>
        /// <param name="obj"></param>
        private async Task DoOTA(object obj)
        {
            try
            {
                int.TryParse(obj.ToString(), out int code);
                var ota = new OTATask
                {
                    Firmware = Info.Code,
                    Type = code,
                    TagIDList = Info.Tags
                };
                var result = await SendService.Instance.Send(0x06, "ota", ota);
                if (result != Demo_Common.Enum.SendResult.Success)
                {
                    MsgHelper.Error("OTA_ERR:" + result);
                }
            }
            catch (Exception ex)
            {
                MsgHelper.Error("OTA_Err:" + ex.Message);
            }
        }

        /// <summary>
        /// Can run
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Result</returns>
        private bool CanRun(object arg) => (Port.Length > 3 && Port.Length < 6) && Info.IP.Length > 0;

        /// <summary>
        /// Can OTA
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Result</returns>
        private bool CanOTA(object arg) => WebService.Instance.IsRun && Info.Tags.Length > 0 && Info.Tags.All(x => x[..2].Equals(Info.Tags[0][..2]));

        /// <summary>
        /// Select tags
        /// </summary>
        /// <param name="tags">Tags</param>
        private void SelectTags(string[] tags)
        {
            Info.Tags = tags;
        }
    }
}