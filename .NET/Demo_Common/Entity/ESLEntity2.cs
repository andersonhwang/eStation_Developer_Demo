using Demo_Common.Enum;
using MessagePack;

namespace Demo_Common.Entity
{
    /// <summary>
    /// Tag entity
    /// </summary>
    [MessagePackObject]
    public class ESLEntity2 : ESLEntity
    {
        [Key(10)]
        public byte[] Bytes { get; set; } = [];
        [Key(11)]
        public bool Compress { get; set; } = true;
    }
}
