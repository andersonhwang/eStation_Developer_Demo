using System.Text.RegularExpressions;

namespace Demo_Common.Enum
{
    public class RegExpress
    {
        public static readonly Regex REG_TAG_ID = new("^[0-9A-F]{12}$");
        public static readonly Regex REG_IP = new("^((25[0-5]|2[0-4]\\d|[10]?\\d?\\d)\\.){3}(25[0-5]|2[0-4]\\d|[10]?\\d?\\d)$");
        public static readonly Regex REG_TAG_KEY = new("^[0-9A-F]{255}$");
    }
}
