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
        /// Period 闪烁时长, 0~3600, Default 3600
        /// </summary>
        [Key(4)]
        public int Period { get; set; } = 3600;
        /// <summary>
        /// Interval 亮灯间隔, 100~10000, Default 1000
        /// </summary>
        [Key(5)]
        public int Interval { get; set; } = 1000;
        /// <summary>
        /// Duration 亮灯时长, 50~100, Default 50
        /// </summary>
        [Key(6)]
        public int Duration { get; set; } = 50;
        /// <summary>
        /// Token
        /// </summary>
        [Key(7)]
        public int Token { get; set; }
        /// <summary>
        /// 图像数据文件
        /// </summary>
        [Key(8)]
        public byte[] HexData { get; set; } = [];
    }
}
