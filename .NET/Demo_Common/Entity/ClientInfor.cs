using Demo_Common.Enum;
using MQTTnet.Server;

namespace Demo_Common.Entity
{
    internal class ClientInfor
    {
        public string ID { get; internal set; }
        public string MAC { get; internal set; } = string.Empty;
        public string Firmware { get; internal set; } = string.Empty;
        public ClientStatus Status { get; internal set; } = ClientStatus.Init;
        public DateTime? ConnectTime { get; internal set; }
        public DateTime? DisconnectTime { get; internal set; }
        public DateTime? SendTime { get; internal set; }
        public DateTime? ReceiveTime { get; internal set; }
        public int TotalCount { get; internal set; } = 0;
        public int SendCount { get; internal set; } = 0;
        public DateTime? HeartbeatTime { get; internal set; }
        public eStationInfor Infor { get; internal set; }

        public ClientInfor(ValidatingConnectionEventArgs args)
        {
            ID = args.ClientId;
            Status = ClientStatus.Online;
            ConnectTime = DateTime.Now;
        }
    }
}
