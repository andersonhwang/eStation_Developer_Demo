using Demo.Enum;
using MessagePack;

namespace Demo_WPF.Model
{
    /// <summary>
    /// Task result entity
    /// </summary>
    [MessagePackObject]
    public class TaskResult
    {
        /// <summary>
        /// COM Port
        /// </summary>
        [Key(0)]
        public int Port { get; set; } = 0;

        /// <summary>
        /// Total count in cache
        /// </summary>
        [Key(1)]
        public int TotalCount { get; set; }

        /// <summary>
        /// Current sending count
        /// </summary>
        [Key(2)]
        public int SendCount { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [Key(3)]
        public MessageCode Message { get; set; } = 0;

        /// <summary>
        /// Tag results list
        /// </summary>
        [Key(4)]
        public List<TagResult> Tags { get; set; } = [];
    }
}