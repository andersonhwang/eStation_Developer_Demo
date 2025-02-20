using Demo_Common.Entity;
using Demo_Common.Enum;
using System.ComponentModel;

namespace Demo_WPF.Model
{
    /// <summary>
    /// AP model
    /// </summary>
    public class ApModel : Ap, INotifyPropertyChanged
    {
        /// <summary>
        /// ID
        /// </summary>
        public string IDM { get => Id; set { Id = value; NotifyPropertyChanged(nameof(IDM)); } }
        /// <summary>
        /// MAC
        /// </summary>
        public string MACM { get => Mac; set { Mac = value; NotifyPropertyChanged(nameof(MACM)); } }
        /// <summary>
        /// Client status
        /// </summary>
        public ApStatus StatusM { get => Status; set { Status = value; NotifyPropertyChanged(nameof(StatusM)); } }
        /// <summary>
        /// EndPoint
        /// </summary>
        public string IPM { get => Ip.ToString(); set { Ip = value; NotifyPropertyChanged(nameof(IPM)); } }
        /// <summary>
        /// Last connect time
        /// </summary>
        public DateTime? ConnectTimeM { get => ConnectTime; set { ConnectTime = value; NotifyPropertyChanged(nameof(ConnectTimeM)); } }
        /// <summary>
        /// Last disconnect time
        /// </summary>
        public DateTime? DisconnectTimeM { get => DisconnectTime; set { DisconnectTime = value; NotifyPropertyChanged(nameof(DisconnectTimeM)); } }
        /// <summary>˚º
        /// Last send time
        /// </summary>
        public DateTime? SendTimeM { get => SendTime; set { SendTime = value; NotifyPropertyChanged(nameof(SendTimeM)); } }
        /// <summary>
        /// Last receive time
        /// </summary>
        public DateTime? ReceiveTimeM { get => ReceiveTime; set { ReceiveTime = value; NotifyPropertyChanged(nameof(ReceiveTimeM)); } }
        /// <summary>
        /// Last heartbeat time
        /// </summary>
        public DateTime? HeartbeatTimeM { get => HeartbeatTime; set { HeartbeatTime = value; NotifyPropertyChanged(nameof(HeartbeatTimeM)); } }
        /// <summary>
        /// Firmware
        /// </summary>
        public string FirmwareM { get => Firmware; set { Firmware = value; NotifyPropertyChanged(nameof(FirmwareM)); } }
        /// <summary>
        /// Wait count
        /// </summary>
        public int WaitCountM { get => WaitCount; set { WaitCount = value; NotifyPropertyChanged(nameof(WaitCountM)); } }
        /// <summary>
        /// Send count
        /// </summary>
        public int SendCountM { get => SendCount; set { SendCount = value; NotifyPropertyChanged(nameof(SendCountM)); } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="ip">AP IP</param>
        /// <param name="status">AP status</param>
        public ApModel(string id, string ip, ApStatus status = ApStatus.Online) : base(id, ip, status)
        { }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
