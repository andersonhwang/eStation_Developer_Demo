using Demo.Enum;
using MessagePack;

namespace Demo_WPF.Model
{
    [MessagePackObject]
    public class eStationMessage
    {
        /// <summary>
        /// Message code
        /// </summary>
        [Key(0)]
        public MessageCode Code { get; set; } = MessageCode.OK;
    }
}