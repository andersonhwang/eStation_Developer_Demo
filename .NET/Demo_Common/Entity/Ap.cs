using Demo_Common.Enum;
using System.Net;

namespace Demo_Common.Entity
{
    public class Ap
    {
        public string Id = string.Empty;
        public string Mac = string.Empty;
        public ApStatus Status = ApStatus.Init;
        public EndPoint? EndPoint = null;
        public string Firmware = string.Empty;
        public int TotalCount = 0;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="endPoint">EndPoint</param>
        /// <param name="status">AP status</param>
        public Ap(string id, EndPoint endPoint, ApStatus status = ApStatus.Online)
        {
            Id = id;
            EndPoint = endPoint;
            Status = status;
        }
    }
}
