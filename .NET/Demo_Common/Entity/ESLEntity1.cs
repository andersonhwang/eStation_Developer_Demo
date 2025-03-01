using MessagePack;

namespace Demo_Common.Entity
{
    /// <summary>
    /// Tag entity
    /// </summary>
    [MessagePackObject]
    public class ESLEntity1 : ESLEntity
    {
        [Key(10)]
        public string Base64String { get; set; } = "";
    }
}
