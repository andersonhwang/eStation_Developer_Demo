using MessagePack;

namespace Demo_Common.Entity
{
    /// <summary>
    /// Tag heartbeat
    /// </summary>
    [MessagePackObject]
    public class TagHeartbeat
    {
        /// <summary>
        /// Tag ID
        /// </summary>
        [Key(0)]
        public string TagId { get; set; } = string.Empty;
        /// <summary>
        /// RF power
        /// </summary>
        [Key(1)]
        public int RfPower { get; set; } = -256;
        /// <summary>
        /// Battery
        /// </summary>
        [Key(2)]
        public byte Battery { get; set; } = 0;
        /// <summary>
        /// Screen
        /// </summary>
        [Key(3)]
        public byte Version { get; set; } = 0;
        /// <summary>
        /// Version
        /// [DSL]01:排线松动屏幕损坏;02:ADC异常;03:分包错误;04:CRC校验错误;05:时间错误
        /// 06:图片尺寸问题;07:图片索引错误(帧头、文件大小等);08:图片压缩数据错误;09:Flash擦除错误
        /// 10:Flash读错误;11:Flash写错误;12:beacon使能异常;13:beacon扫描异常;14:没有LED灯(暂时无法使用)
        /// [ESL]00:No error;01:LCM ID error;02:MCU reset or No LCM;03:LCM refresh error
        /// </summary>
        [Key(4)]
        public byte Status { get; set; } = 0;
        /// <summary>
        /// Token
        /// </summary>
        [Key(5)]
        public int Token { get; set; } = 0;
        /// <summary>
        /// Temperature
        /// </summary>
        [Key(6)]
        public int Temperature { get; set; } = 0;
        /// <summary>
        /// Channel
        /// </summary>
        [Key(7)]
        public int Channel { get; set; } = 0;
        /// <summary>
        /// [DSL]UTC time, 
        /// </summary>
        [Key(8)]
        public byte[] UtcTime { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// [DSL]UTC time, 
        /// </summary>
        [Key(9)]
        public byte TimePercent { get; set; } = 0;
        /// <summary>
        /// [DSL]Count
        /// </summary>
        [Key(10)]
        public byte Count { get; set; } = 0;
        /// <summary>
        /// [ESL]Factory code
        /// </summary>
        [Key(11)]
        public byte Factory { get; set; } = 0;
        /// <summary>
        /// [ESL]Color code
        /// </summary>
        [Key(12)]
        public byte Color { get; set; } = 0;
        /// <summary>
        /// [ESL]Screen size code
        /// </summary>
        [Key(13)]
        public byte Size { get; set; } = 0;
        /// <summary>
        /// [ESL]Type code
        /// </summary>
        [Key(14)]
        public byte Type { get; set; } = 0;
    }
}
