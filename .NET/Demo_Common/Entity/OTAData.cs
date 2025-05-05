using MessagePack;

namespace Demo_Common.Entity
{
    /// <summary>
    /// OTA Data
    /// </summary>
    [MessagePackObject]
    public class OTAData
    {
        /// <summary>
        /// Firmware donwload url
        /// </summary>
        [Key(0)]
        public string DownloadUrl { get; set; } = string.Empty;
        /// <summary>
        /// Firmware download confirm url
        /// </summary>
        [Key(1)]
        public string ConfirmUrl { get; set; } = string.Empty;
        /// <summary>
        /// OTA type: 1-eStation_Developer_Edition, 2-Mod_Data, 3-Mod_Heartbeat, 4-ESL, 5-DSL
        /// </summary>
        [Key(2)]
        public int Type { get; set; } = 1;
        /// <summary>
        /// Version
        /// </summary>
        [Key(3)]
        public string Version { get; set; } = string.Empty;
        /// <summary>
        /// Firmare name
        /// </summary>
        [Key(4)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// MD5 Check String, 
        /// Version<=1.0.24, 00-11-22-33-44-55-66-77
        /// Version>=1.0.25, 0011223344556677
        /// </summary>
        [Key(5)]
        public string MD5 { get; set; } = string.Empty;
    }
}
