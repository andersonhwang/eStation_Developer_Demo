using Demo_Common.Enum;
using MessagePack;

namespace Demo_Common.Entity
{
    [MessagePackObject]
    public class ApMessage
    {
        /// <summary>
        /// Message code
        /// </summary>
        [Key(0)]
        public MessageCode Code { get; set; } = MessageCode.OK;
    }
}