using Demo_Common.Entity;
using Demo_Common.Enum;
using Serilog;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Demo_Common.Helper
{
    /// <summary>
    /// Tag helper
    /// </summary>
    public static class TagHelper
    {
        private static readonly string FilePath = "TagType.json";
        private static readonly Dictionary<string, TagType> DicTypes = new();
        public static readonly Regex RegTagID = new("^[0-9A-F]{12}$");

        /// <summary>
        /// Constructor
        /// </summary>
        static TagHelper()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var data = File.ReadAllText(FilePath);
                    var items = JsonSerializer.Deserialize<List<TagType>>(data);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            var code = item.Code.Trim().ToUpper();
                            if (!DicTypes.ContainsKey(code))
                            {
                                DicTypes.Add(code, item);
                            }
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Init_PB_Error");
            }
            finally
            {
                if (DicTypes.Count == 0)
                {
                    LoadDefault(); // Load default
                    File.WriteAllText(FilePath, JsonSerializer.Serialize(DicTypes.Values.ToList()));
                    Log.Warning("Load_Default_ESL_Type:" + DicTypes.Count);
                }
            }
        }

        /// <summary>
        /// Tag types
        /// </summary>
        public static List<TagType> TagTypes { get => DicTypes.Values.ToList(); }

        /// <summary>
        /// Get tag type
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public static TagType GetTagType(string tagId)
        {
            tagId = tagId.Trim().ToUpper();
            if (RegTagID.IsMatch(tagId))
                if (DicTypes.ContainsKey(tagId[..2])) return DicTypes[tagId[..2]];
            return new TagType(string.Empty, 0, 0, 0, ColorType.Black);
        }

        /// <summary>
        /// Load default
        /// </summary>
        private static void LoadDefault()
        {
            DicTypes.Add("30", new TagType("30", 152, 152, 154, ColorType.BlackRed));
            DicTypes.Add("33", new TagType("33", 200, 200, 154, ColorType.BlackRed));
            DicTypes.Add("36", new TagType("36", 250, 122, 213, ColorType.BlackRed));
            DicTypes.Add("39", new TagType("39", 250, 122, 213, ColorType.Black));
            DicTypes.Add("3A", new TagType("3A", 296, 152, 266, ColorType.BlackRed));
            DicTypes.Add("3D", new TagType("3D", 296, 128, 290, ColorType.BlackRed));
            DicTypes.Add("40", new TagType("40", 400, 300, 420, ColorType.BlackRed));
            DicTypes.Add("42", new TagType("42", 400, 300, 420, ColorType.Black));
            DicTypes.Add("43", new TagType("43", 400, 300, 420, ColorType.BlackRed));
            DicTypes.Add("44", new TagType("44", 800, 480, 750, ColorType.BlackRed));
            DicTypes.Add("49", new TagType("49", 960, 640, 1160, ColorType.BlackRed));
            DicTypes.Add("4C", new TagType("4C", 522, 152, 430, ColorType.BlackRed));
            DicTypes.Add("4F", new TagType("4F", 648, 480, 580, ColorType.BlackRed));
            DicTypes.Add("54", new TagType("54", 296, 128, 290, ColorType.Black));
            DicTypes.Add("55", new TagType("55", 384, 184, 350, ColorType.BlackRed));
            DicTypes.Add("58", new TagType("58", 1304, 984, 1250, ColorType.BlackRed));
            DicTypes.Add("5B", new TagType("5B", 296, 152, 266, ColorType.Black));
            DicTypes.Add("5D", new TagType("5D", 400, 300, 420, ColorType.BlackRed));
            DicTypes.Add("64", new TagType("64", 960, 640, 1020, ColorType.BlackRed));
            DicTypes.Add("68", new TagType("68", 960, 680, 1330, ColorType.BlackRed));
            DicTypes.Add("80", new TagType("80", 200, 200, 154, ColorType.BlackRedYellow));
            DicTypes.Add("81", new TagType("81", 250, 122, 213, ColorType.BlackRedYellow));
            DicTypes.Add("82", new TagType("82", 296, 152, 266, ColorType.BlackRedYellow));
            DicTypes.Add("84", new TagType("84", 296, 128, 290, ColorType.BlackRedYellow));
            DicTypes.Add("86", new TagType("86", 384, 184, 350, ColorType.BlackRedYellow));
            DicTypes.Add("87", new TagType("87", 400, 300, 420, ColorType.BlackRedYellow));
            DicTypes.Add("88", new TagType("88", 648, 480, 580, ColorType.BlackRedYellow));
            DicTypes.Add("89", new TagType("89", 800, 480, 750, ColorType.BlackRedYellow));
            DicTypes.Add("8B", new TagType("8B", 960, 640, 1020, ColorType.BlackRedYellow));
            DicTypes.Add("8E", new TagType("8E", 960, 680, 1330, ColorType.BlackRedYellow));
            DicTypes.Add("8F", new TagType("8F", 416, 240, 370, ColorType.BlackRedYellow));
            DicTypes.Add("91", new TagType("91", 522, 152, 430, ColorType.BlackRedYellow));
            DicTypes.Add("95", new TagType("95", 250, 122, 213, (ColorType)8));
            DicTypes.Add("96", new TagType("96", 296, 152, 260, (ColorType)8));
            DicTypes.Add("98", new TagType("98", 384, 184, 350, (ColorType)8));
            DicTypes.Add("99", new TagType("99", 792, 272, 600, ColorType.BlackRedYellow));
            DicTypes.Add("A0", new TagType("A0", 800, 480, 730, (ColorType)9));
            DicTypes.Add("A1", new TagType("A1", 1600, 1200, 1330, (ColorType)9));
            DicTypes.Add("A2", new TagType("A2", 250, 122, 213, (ColorType)10));
            DicTypes.Add("A3", new TagType("A3", 1120, 784, 970, (ColorType)9));
        }
    }
}
