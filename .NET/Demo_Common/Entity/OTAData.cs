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
        /// MD5 Check String
        /// </summary>
        [Key(5)]
        public string MD5 { get; set; } = string.Empty;
    }
}
