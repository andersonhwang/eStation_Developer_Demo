using System;
using eStationDemo.Enum;
using MQTTnet.Server;
using ReactiveUI;

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
        public string ID { get => id; set => this.RaiseAndSetIfChanged(ref id, value); } 
        /// <summary>
        /// MAC
        /// </summary>
        public string MAC { get => mac; set => this.RaiseAndSetIfChanged(ref mac, value); } 
        /// <summary>
        /// Client status
        /// </summary>
        public ClientStatus Status { get => status; set => this.RaiseAndSetIfChanged(ref status, value); } 
        /// <summary>
        /// EndPoint
        /// </summary>
        public string EndPoint { get => endPoint; set => this.RaiseAndSetIfChanged(ref endPoint, value); } 
        /// <summary>
        /// Last connect time
        /// </summary>
        public DateTime? ConnectTime { get => connectTime; set => this.RaiseAndSetIfChanged(ref connectTime, value); } 
        /// <summary>
        /// Last disconnect time
        /// </summary>
        public DateTime? DisconnectTime { get => disconnectTime; set => this.RaiseAndSetIfChanged(ref disconnectTime, value); } 
        /// <summary>˚º
        /// Last send time
        /// </summary>
        public DateTime? SendTime { get => sendTime; set => this.RaiseAndSetIfChanged(ref sendTime, value); } 
        /// <summary>
        /// Last receive time
        /// </summary>
        public DateTime? ReceiveTime { get => receiveTime; set => this.RaiseAndSetIfChanged(ref receiveTime, value); } 
        /// <summary>
        /// Last heartbeat time
        /// </summary>
        public DateTime? HeartbeatTime { get => heartbeatTime; set => this.RaiseAndSetIfChanged(ref heartbeatTime, value); } 
        /// <summary>
        /// Firmware
        /// </summary>
        public string Firmware { get => firmware; set => this.RaiseAndSetIfChanged(ref firmware, value); } 
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get => totalCount; set => this.RaiseAndSetIfChanged(ref totalCount, value); } 

        /// <summary>
        /// Send count
        /// </summary>
        public int SendCount { get => sendCount; set => this.RaiseAndSetIfChanged(ref sendCount, value); } 

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