using MessagePack;
using PageIndex = Demo_Common.Enum.PageIndex;
using Pattern = Demo_Common.Enum.Pattern;

namespace Demo_Common.Entity
{
    /// <summary>
    /// Tag entity
    /// </summary>
    [MessagePackObject]
    public class ESLEntity
    {
        [Key(0)]
        public string TagID { get; set; } = "";
        [Key(1)]
        public Pattern Pattern { get; set; } = Pattern.UpdateDisplay;
        [Key(2)]
        public PageIndex PageIndex { get; set; } = PageIndex.P0;
        [Key(3)]
        public bool R { get; set; } = false;
        [Key(4)]
        public bool G { get; set; } = false;
        [Key(5)]
        public bool B { get; set; } = false;
        [Key(6)]
        public int Times { get; set; } = 0;
        [Key(7)]
        public int Token { get; set; } = 0;
        [Key(8)]
        public string OldKey { get; set; } = "";
        [Key(9)]
        public string NewKey { get; set; } = "";
        [Key(10)]
        public string Base64String { get; set; } = "";
    }
}
