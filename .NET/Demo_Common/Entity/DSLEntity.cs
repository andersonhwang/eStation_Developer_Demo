using MessagePack;

namespace Demo_Common.Entity
{
    [MessagePackObject]
    public class DSLEntity
    {
        /// <summary>
        /// Tag ID
        /// </summary>
        [Key(0)]
        public string TagID { get; set; } = "";
        /// <summary>
        /// LED - Red
        /// </summary>
        [Key(1)]
        public bool R { get; set; }
        /// <summary>
        /// LED - Green
        /// </summary>
        [Key(2)]
        public bool G { get; set; }
        /// <summary>
        /// LED - Blue
        /// </summary>
        [Key(3)]
        public bool B { get; set; }
        /// <summary>
        /// Times
        /// </summary>
        [Key(4)]
        public int Times { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        [Key(5)]
        public int Token { get; set; }
        /// <summary>
        /// Interval 亮灯间隔
        /// </summary>
        [Key(6)]
        public int Interval { get; set; } = 0;
        /// <summary>
        /// Duration 亮灯时长
        /// </summary>
        [Key(7)]
        public int Duration { get; set; } = 50;
        /// <summary>
        /// 图像数据文件
        /// </summary>
        [Key(8)]
        public byte[] HexData { get; set; } = Array.Empty<byte>();
    }
}
