using MessagePack;

namespace Demo_Common.Entity
{
    /// <summary>
    /// AP information
    /// </summary>
    [MessagePackObject]
    public class eStationInfor
    {
        /// <summary>
        /// ID: Code4 
        /// </summary>
        [Key(0)]
        public string ID { get; set; } = "0000";
        /// <summary>
        /// Alias
        /// </summary>
        [Key(1)]
        public string Alias { get; set; } = "";
        /// <summary>
        /// Local IP
        /// </summary>
        [Key(2)]
        public string IP { get; set; } = "127.0.0.1";
        /// <summary>
        /// MAC, refer ID
        /// </summary>
        [Key(3)]
        public string MAC { get; set; } = "90A9F7300000";
        /// <summary>
        /// AP type
        /// </summary>
        [Key(4)]
        public int ApType { get; set; } = 3; // AP05 = 3;
        /// <summary>
        /// Firmware version
        /// </summary>
        [Key(5)]
        public string ApVersion { get; set; } = "1.0.1";
        /// <summary>
        /// MOD ModVersion
        /// </summary>
        [Key(6)]
        public string ModVersion { get; set; } = "";
        /// <summary>
        /// Disk size (MB)
        /// </summary>
        [Key(7)]
        public int DiskSize { get; set; } = 0;
        /// <summary>
        /// Disk free space (MB)
        /// </summary>
        [Key(8)]
        public int FreeSpace { get; set; } = 0;
        /// <summary>
        /// Server address
        /// </summary>
        [Key(9)]
        public string Server { get; set; } = "192.168.4.92";
        /// <summary>
        /// Connection parameters
        /// </summary>
        [Key(10)]
        public string[] ConnParam { get; set; } = [];
        /// <summary>
        /// Encrypt with SslProtocol.Tls12
        /// </summary>
        [Key(11)]
        public bool Encrypt { get; set; } = false;
        /// <summary>
        /// Auto network
        /// </summary>
        [Key(12)]
        public bool AutoIP { get; set; } = true;
        /// <summary>
        /// Local IP, empty if auto network
        /// </summary>
        [Key(13)]
        public string LocalIP { get; set; } = string.Empty;
        /// <summary>
        /// Subnet mask, empty if auto network
        /// </summary>
        [Key(14)]
        public string Subnet { get; set; } = string.Empty;
        /// <summary>
        /// Gateway, empty if auto network
        /// </summary>
        [Key(15)]
        public string Gateway { get; set; } = string.Empty;
        /// <summary>
        /// Heartbeat speed
        /// </summary>
        [Key(16)]
        public int Heartbeat { get; set; } = 15;
    }
}