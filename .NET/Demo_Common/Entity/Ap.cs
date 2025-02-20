using Demo_Common.Enum;

namespace Demo_Common.Entity
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="id">AP ID</param>
    /// <param name="ip">EndPoint</param>
    /// <param name="status">AP status</param>
    public class Ap(string id, string ip, ApStatus status = ApStatus.Online)
    {
        public string Id = id;
        public string Mac = string.Empty;
        public ApStatus Status = status;
        public string Ip = ip;
        public string Firmware = string.Empty;
        public int WaitCount = 0;
        public int SendCount = 0;
        public DateTime? ConnectTime = null;
        public DateTime? DisconnectTime = null;
        public DateTime? SendTime = null;
        public DateTime? ReceiveTime = null;
        public DateTime? HeartbeatTime = null;

        /// <summary>
        /// AP information
        /// </summary>
        public ApInfor Infor { get; set; } = new ApInfor();
    }
}
