using Demo_Common.Entity;
using Demo_Common.Service;
using Demo_WPF.Model;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class ApInforViewModel : ViewModelBase
    {
        private ApModel? ap;
        /// <summary>
        /// AP infor
        /// </summary>
        public ApModel? AP { get { return ap; } set { ap = value; NotifyPropertyChanged(nameof(AP)); } }

        public ICommand CmdConfig { get; set; }
        public ICommand CmdOTA { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApInforViewModel()
        {
            CmdConfig = new MyCommand(DoConfig, CanConfig);
            CmdOTA = new MyCommand(DoOTA, CanOTA);

            SendService.Instance.Register(SetApInfor);
        }

        /// <summary>
        /// Set AP infor
        /// </summary>
        /// <param name="ap">AP</param>
        public void SetApInfor(Ap ap)
        {
            AP = new ApModel(ap.Id, ap.Ip, ap.Status)
            {
                MACM = ap.Mac,
                FirmwareM = ap.Firmware,
                SendCountM = ap.SendCount,
                WaitCountM = ap.WaitCount,
                HeartbeatTimeM = ap.HeartbeatTime,
                SendTimeM = ap.SendTime,
                ReceiveTimeM = ap.ReceiveTime,
                ConnectTimeM = ap.ConnectTime,
                DisconnectTimeM = ap.DisconnectTime
            };
        }

        /// <summary>
        /// Can OTA
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanOTA(object arg) => IsConnect;

        /// <summary>
        /// Can config
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanConfig(object arg) => IsConnect;

        /// <summary>
        /// Do OTA
        /// </summary>
        /// <param name="obj"></param>
        private void DoOTA(object obj)
        {
            var ota = new OTAWindow();
            ota.ShowDialog();
        }

        /// <summary>
        /// Do config
        /// </summary>
        /// <param name="obj"></param>
        private void DoConfig(object obj)
        {
            var config = new ConfigWindow(ap.Id, ap);
            config.ShowDialog();
        }
    }
}
