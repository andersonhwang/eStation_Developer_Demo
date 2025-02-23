using Demo_Common.Enum;
using MessagePack;

namespace Demo_Common.Entity
{
    /// <summary>
    /// AP heartbeat data
    /// </summary>
    [MessagePackObject]
    public class ApHeartbeat
    {
        /// <summary>
        /// AP ID
        /// </summary>
        [Key(0)]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Config version
        /// </summary>
        [Key(1)]
        public int ConfigVersion { get; set; }
        /// <summary>
        /// Ap version
        /// </summary>
        [Key(2)]
        public string ApVersion { get; set; } = "0.0.0";
        /// <summary>
        /// Mod version
        /// </summary>
        [Key(3)]
        public string ModVersion { get; set; } = string.Empty;
        /// <summary>
        /// Message
        /// </summary>
        [Key(4)]
        public MessageCode Message { get; set; } = MessageCode.OK;
        /// <summary>
        /// External message
        /// </summary>
        [Key(5)]
        public string MessageEx { get; set; } = string.Empty;
        /// <summary>
        /// Total count in cache
        /// </summary>
        [Key(6)]
        public int WaitCount { get; set; }
        /// <summary>
        /// Current sending count
        /// </summary>
        [Key(7)]
        public int SendCount { get; set; }
        /// <summary>
        /// Tag results list
        /// </summary>
        [Key(8)]
        public List<TagHeartbeat> Tags { get; set; } = new List<TagHeartbeat>();
    }
}
