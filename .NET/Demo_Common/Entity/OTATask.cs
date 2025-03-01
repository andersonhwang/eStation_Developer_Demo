using MessagePack;

namespace Demo_Common.Entity
{
    [MessagePackObject]
    public class OTATask
    {
        /// <summary>
        /// OTA Type 0/1/2
        /// </summary>
        [Key(0)]
        public int Type { get; set; } = 0;
        /// <summary>
        /// Firmware
        /// </summary>
        [Key(1)]
        public string Firmware { get; set; } = string.Empty;
        /// <summary>
        /// Tags to OTA
        /// </summary>
        [Key(2)]
        public string[] TagIDList { get; set; } = Array.Empty<string>();
    }
}
