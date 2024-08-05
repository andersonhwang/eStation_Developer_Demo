using System;
using eStationDemo.Enum;
using MQTTnet.Server;

namespace eStationDemo.Model
{
    /// <summary>
    /// Client information
    /// </summary>
    public class ClientInfor : ModelBase
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get => id; set { id = value; NotifyPropertyChanged(nameof(ID)); } }
        /// <summary>
        /// MAC
        /// </summary>
        public string MAC { get => mac; set { mac = value; NotifyPropertyChanged(nameof(MAC)); } }
        /// <summary>
        /// Client status
        /// </summary>
        public ClientStatus Status { get => status; set { status = value; NotifyPropertyChanged(nameof(Status)); } }
        /// <summary>
        /// EndPoint
        /// </summary>
        public string EndPoint { get => endPoint; set { endPoint = value; NotifyPropertyChanged(nameof(EndPoint)); } }
        /// <summary>
        /// Last connect time
        /// </summary>
        public DateTime? ConnectTime { get => connectTime; set { connectTime = value; NotifyPropertyChanged(nameof(ConnectTime)); } }
        /// <summary>
        /// Last disconnect time
        /// </summary>
        public DateTime? DisconnectTime { get => disconnectTime; set { disconnectTime = value; NotifyPropertyChanged(nameof(DisconnectTime)); } }
        /// <summary>˚º
        /// Last send time
        /// </summary>
        public DateTime? SendTime { get => sendTime; set { sendTime = value; NotifyPropertyChanged(nameof(SendTime)); } }
        /// <summary>
        /// Last receive time
        /// </summary>
        public DateTime? ReceiveTime { get => receiveTime; set { receiveTime = value; NotifyPropertyChanged(nameof(ReceiveTime)); } }
        /// <summary>
        /// Last heartbeat time
        /// </summary>
        public DateTime? HeartbeatTime { get => heartbeatTime; set { heartbeatTime = value; NotifyPropertyChanged(nameof(HeartbeatTime)); } }
        /// <summary>
        /// Firmware
        /// </summary>
        public string Firmware { get => firmware; set { firmware = value; NotifyPropertyChanged(nameof(Firmware)); } }
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get => totalCount; set { totalCount = value; NotifyPropertyChanged(nameof(TotalCount)); } }

        /// <summary>
        /// Send count
        /// </summary>
        public int SendCount { get => sendCount; set { sendCount = value; NotifyPropertyChanged(nameof(SendCount)); } }

        /// <summary>
        /// eStation information
        /// </summary>
        public eStationInfor Infor { get; set; } = new eStationInfor();

        string id = string.Empty;
        string mac = string.Empty;
        ClientStatus status = ClientStatus.Init;
        string endPoint = string.Empty;
        string firmware = string.Empty;
        int totalCount = 0;
        int sendCount = 0;
        DateTime? connectTime = null;
        DateTime? disconnectTime = null;
        DateTime? sendTime = null;
        DateTime? receiveTime = null;
        DateTime? heartbeatTime = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args">Event</param>
        public ClientInfor(ValidatingConnectionEventArgs args)
        {
            ID = args.ClientId;
            MAC = args.ClientId;
            EndPoint = args.Endpoint;
            ConnectTime = DateTime.Now;
        }
    }
}