using Demo_Common.Enum;
using MessagePack;

namespace Demo_Common.Entity
{
    [MessagePackObject]
    public class DSLEntity2 : DSLEntity
    {
        /// <summary>
        /// Pattern
        /// </summary>
        [Key(9)]
        public Pattern Pattern { get; set; }
        /// <summary>
        /// Current key
        /// </summary>
        [Key(10)]
        public string CurrentKey { get; set; } = "";
        /// <summary>
        /// New key
        /// </summary>
        [Key(11)]
        public string NewKey { get; set; } = "";
    }
}
