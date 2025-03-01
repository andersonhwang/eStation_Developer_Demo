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
            return new TagType(string.Empty, 0, 0, "", ColorType.Black);
        }

        /// <summary>
        /// Load default
        /// </summary>
        private static void LoadDefault()
        {
            DicTypes.Add("30", new TagType("30", 152, 152, "ET0154-30", ColorType.BlackRed));
            DicTypes.Add("31", new TagType("31", 152, 152, "ET0154-31", ColorType.BlackYellow));
            DicTypes.Add("32", new TagType("32", 152, 152, "ET0154-32", ColorType.Black));
            DicTypes.Add("33", new TagType("33", 200, 200, "ET0154-33", ColorType.BlackRed));
            DicTypes.Add("34", new TagType("34", 200, 200, "ET0154-34", ColorType.BlackYellow));
            DicTypes.Add("35", new TagType("35", 200, 200, "ET0154-35", ColorType.Black));
            DicTypes.Add("36", new TagType("36", 250, 122, "ET0213-36", ColorType.BlackRed));
            DicTypes.Add("37", new TagType("37", 250, 250, "ET0213-37", ColorType.BlackYellow));
            DicTypes.Add("38", new TagType("38", 250, 122, "ET0213-38", ColorType.Black));
            DicTypes.Add("39", new TagType("39", 250, 122, "ET0213-39", ColorType.Black));
            DicTypes.Add("3A", new TagType("3A", 296, 152, "ET0266-3A", ColorType.BlackRed));
            DicTypes.Add("3B", new TagType("3B", 296, 152, "ET0266-3B", ColorType.BlackYellow));
            DicTypes.Add("3C", new TagType("3C", 296, 152, "ET0266-3C", ColorType.Black));
            DicTypes.Add("3D", new TagType("3D", 296, 128, "ET0290-3D", ColorType.BlackRed));
            DicTypes.Add("3E", new TagType("3E", 296, 296, "ET0290-3E", ColorType.BlackYellow));
            DicTypes.Add("3F", new TagType("3F", 296, 128, "ET0290-3F", ColorType.Black));
            DicTypes.Add("40", new TagType("40", 400, 300, "ET0420-40", ColorType.Black));
            DicTypes.Add("41", new TagType("41", 400, 300, "ET0420-41", ColorType.BlackYellow));
            DicTypes.Add("42", new TagType("42", 400, 300, "ET0420-42", ColorType.Black));
            DicTypes.Add("43", new TagType("43", 400, 300, "ET0420-43", ColorType.BlackRed));
            DicTypes.Add("44", new TagType("44", 800, 480, "ET0750-44", ColorType.BlackRed));
            DicTypes.Add("45", new TagType("45", 800, 800, "ET0750-45", ColorType.BlackYellow));
            DicTypes.Add("46", new TagType("46", 800, 480, "ET0750-46", ColorType.Black));
            DicTypes.Add("47", new TagType("47", 800, 480, "ET0750-47", ColorType.BlackRed));
            DicTypes.Add("48", new TagType("48", 800, 480, "ET0750-48", ColorType.BlackRed));
            DicTypes.Add("49", new TagType("49", 960, 640, "ET1160-49", ColorType.BlackRed));
            DicTypes.Add("4A", new TagType("4A", 960, 640, "ET1160-4A", ColorType.BlackYellow));
            DicTypes.Add("4B", new TagType("4B", 960, 640, "ET1160-4B", ColorType.Black));
            DicTypes.Add("4C", new TagType("4C", 522, 122, "ET0430-4C", ColorType.BlackRed));
            DicTypes.Add("4D", new TagType("4D", 522, 122, "ET0430-4D", ColorType.BlackYellow));
            DicTypes.Add("4E", new TagType("4E", 522, 122, "ET0430-4E", ColorType.Black));
            DicTypes.Add("4F", new TagType("4F", 648, 480, "ET0580-4F", ColorType.BlackRed));
            DicTypes.Add("50", new TagType("50", 648, 480, "ET0580-50", ColorType.BlackYellow));
            DicTypes.Add("51", new TagType("51", 648, 480, "ET0580-51", ColorType.Black));
            DicTypes.Add("54", new TagType("54", 296, 128, "ET0290-54", ColorType.Black));
            DicTypes.Add("55", new TagType("55", 384, 184, "ET0350-55", ColorType.BlackRed));
            DicTypes.Add("56", new TagType("56", 384, 184, "ET0350-56", ColorType.BlackYellow));
            DicTypes.Add("57", new TagType("57", 384, 184, "ET0350-57", ColorType.Black));
            DicTypes.Add("58", new TagType("58", 1304, 984, "ET1250-58", ColorType.BlackRed));
            DicTypes.Add("59", new TagType("59", 1304, 984, "ET1250-59", ColorType.BlackYellow));
            DicTypes.Add("5A", new TagType("5A", 1304, 984, "ET1250-5A", ColorType.Black));
            DicTypes.Add("5B", new TagType("5B", 296, 152, "ET0266-5B", ColorType.Black));
            DicTypes.Add("5D", new TagType("5D", 400, 300, "ET0420-5D", ColorType.BlackRed));
            DicTypes.Add("5F", new TagType("5F", 800, 480, "ET0730-5F", ColorType.BlackRedYellow));        // 7-Color
            DicTypes.Add("62", new TagType("62", 200, 144, "ET0130-62", ColorType.BlackRed));
            DicTypes.Add("64", new TagType("64", 960, 640, "ET1020-64", ColorType.BlackRed));
            DicTypes.Add("67", new TagType("67", 960, 640, "ET1020-67", ColorType.Black));
            DicTypes.Add("68", new TagType("68", 960, 680, "ET1330-68", ColorType.BlackRed));
            DicTypes.Add("69", new TagType("69", 184, 88, "ET0097-69", ColorType.BlackRed));
            DicTypes.Add("6F", new TagType("6F", 792, 272, "ET0579-6F", ColorType.BlackRed));
            DicTypes.Add("80", new TagType("80", 200, 200, "ET0154-80", ColorType.BlackRedYellow));
            DicTypes.Add("81", new TagType("81", 250, 122, "ET0213-81", ColorType.BlackRedYellow));
            DicTypes.Add("82", new TagType("82", 296, 152, "ET0266-82", ColorType.BlackRedYellow));
            DicTypes.Add("83", new TagType("83", 360, 184, "ET0266-83", ColorType.BlackRedYellow));
            DicTypes.Add("84", new TagType("84", 296, 128, "ET0290-84", ColorType.BlackRedYellow));
            DicTypes.Add("85", new TagType("85", 384, 168, "ET0290-85", ColorType.BlackRedYellow));
            DicTypes.Add("86", new TagType("86", 384, 184, "ET0350-86", ColorType.BlackRedYellow));
            DicTypes.Add("87", new TagType("87", 400, 300, "ET0420-87", ColorType.BlackRedYellow));
            DicTypes.Add("88", new TagType("88", 648, 480, "ET0580-88", ColorType.BlackRedYellow));
            DicTypes.Add("89", new TagType("89", 800, 480, "ET0750-89", ColorType.BlackRedYellow));
            DicTypes.Add("8A", new TagType("8A", 880, 528, "ET0750-8A", ColorType.BlackRedYellow));
            DicTypes.Add("8B", new TagType("8B", 960, 640, "ET1020-8B", ColorType.BlackRedYellow));
            DicTypes.Add("D0", new TagType("D0", 960, 640, "ET0240C-D0", ColorType.Black));
        }
    }
}
