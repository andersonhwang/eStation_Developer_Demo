using MessagePack;
using PageIndex = Demo.Enum.PageIndex;
using Pattern = Demo.Enum.Pattern;

namespace Demo_WPF.Model
{
    /// <summary>
    /// ESL entity
    /// </summary>
    [MessagePackObject]
    public class ESLEntity
    {
        [Key(0)]
        public string TagID { get; set; } = "";
        [Key(1)]
        public Pattern Pattern { get; set; }
        [Key(2)]
        public PageIndex PageIndex { get; set; }
        [Key(3)]
        public bool R { get; set; }
        [Key(4)]
        public bool G { get; set; }
        [Key(5)]
        public bool B { get; set; }
        [Key(6)]
        public int Times { get; set; }
        [Key(7)]
        public int Token { get; set; }
        [Key(8)]
        public string OldKey { get; set; }
        [Key(9)]
        public string NewKey { get; set; }
        [Key(10)]
        public string Base64String { get; set; } = "";
    }
}
