using MessagePack;

namespace Demo_Common.Entity
{
    [MessagePackObject]
    public class ApConfig
    {
        /// <summary>
        /// Alias
        /// </summary>
        [Key(0)]
        public string Alias { get; set; } = "";
        /// <summary>
        /// Server address
        /// </summary>
        [Key(1)]
        public string Server { get; set; } = "192.168.4.92";
        /// <summary>
        /// Connection parameters
        /// </summary>
        [Key(2)]
        public string[] ConnParam { get; set; } = ["test", "123456"];
        /// <summary>
        /// Encrypt with SslProtocol.Tls12
        /// </summary>
        [Key(3)]
        public bool Encrypt { get; set; } = false;
        /// <summary>
        /// Auto network
        /// </summary>
        [Key(4)]
        public bool AutoIP { get; set; } = true;
        /// <summary>
        /// Local IP, empty if auto network
        /// </summary>
        [Key(5)]
        public string LocalIP { get; set; } = string.Empty;
        /// <summary>
        /// Subnet mask, empty if auto network
        /// </summary>
        [Key(6)]
        public string Subnet { get; set; } = string.Empty;
        /// <summary>
        /// Gateway, empty if auto network
        /// </summary>
        [Key(7)]
        public string Gateway { get; set; } = string.Empty;
        /// <summary>
        /// Heartbeat speed
        /// </summary>
        [Key(8)]
        public int Heartbeat { get; set; } = 15;
    }
}