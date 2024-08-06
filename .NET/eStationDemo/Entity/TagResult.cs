using MessagePack;

namespace eStationDemo.Model
{
    [MessagePackObject]
    /// <summary>
    /// Tag result
    /// </summary>
    public class TagResult
    {
        /// <summary>
        /// Tag ID
        /// </summary>
        [Key(0)]
        public string TagID { get; set; } = string.Empty;
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
        public byte[] UtcTime { get; set; } = [];
        /// <summary>
        /// [DSL]UTC time, 
        /// </summary>
        [Key(9)]
        public byte[] TimePercent { get; set; } = [];
        /// <summary>
        /// [DSL]Count
        /// </summary>
        [Key(10)]
        public byte Count { get; set; } = 0;
    }
}