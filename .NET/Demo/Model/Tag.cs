using Demo_Common.Enum;

namespace Demo_WPF.Model
{
    internal class Tag : ModelBase
    {
        private string id = string.Empty;
        private TagStatus status = TagStatus.Init;
        private DateTime? lastSend = null;
        private DateTime? lastRecv = null;
        private DateTime? lastHeartbeat = null;
        private int? battery = 0;
        private int? rfPower = -256;
        private int? temperature = 0;
        private int sendCount = 0;
        private int errorCount = 0;
        private int heartbeatCount = 0;

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get => id; set { id = value; NotifyPropertyChanged(nameof(ID)); } }
        /// <summary>
        /// Status
        /// </summary>
        public TagStatus Status { get => status; set { status = value; NotifyPropertyChanged(nameof(Status)); } }
        /// <summary>
        /// Last send
        /// </summary>
        public DateTime? LastSend { get => lastSend; set { lastSend = value; NotifyPropertyChanged(nameof(LastSend)); } }
        /// <summary>
        /// Last receive
        /// </summary>
        public DateTime? LastRecv { get => lastRecv; set { lastRecv = value; NotifyPropertyChanged(nameof(LastRecv)); } }
        /// <summary>
        /// Last heartbeat
        /// </summary>
        public DateTime? LastHeartbeat { get => lastHeartbeat; set { lastHeartbeat = value; NotifyPropertyChanged(nameof(LastHeartbeat)); } }
        /// <summary>
        /// Battery
        /// </summary>
        public int? Battery { get => battery; set { battery = value; NotifyPropertyChanged(nameof(Battery)); } }
        /// <summary>
        /// RF power
        /// </summary>
        public int? RfPower { get => rfPower; set { rfPower = value; NotifyPropertyChanged(nameof(RfPower)); } }
        /// <summary>
        /// Temperature
        /// </summary>
        public int? Temperature { get => temperature; set { temperature = value; NotifyPropertyChanged(nameof(Temperature)); } }
        /// <summary>
        /// Send count
        /// </summary>
        public int SendCount { get => sendCount; set { sendCount = value; NotifyPropertyChanged(nameof(SendCount)); } }
        /// <summary>
        /// Error count
        /// </summary>
        public int ErrorCount { get => errorCount; set { errorCount = value; NotifyPropertyChanged(nameof(ErrorCount)); } }
        /// <summary>
        /// Heartbeat count
        /// </summary>
        public int HeartbeatCount { get => heartbeatCount; set { heartbeatCount = value; NotifyPropertyChanged(nameof(HeartbeatCount)); } }
    }
}
