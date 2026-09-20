using Demo_Common.Enum;
using SkiaSharp;
using System.Text.Json.Serialization;

namespace Demo_Common.Entity
{
    /// <summary>
    /// Tag type
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    /// <param name="code">Type code</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="size">Size</param>
    /// <param name="color">Color type</param>
    public class TagType(string code, int width, int height, int size, ColorType color = ColorType.BlackRed)
    {
        /// <summary>
        /// Tag code
        /// </summary>
        public string Code { get; private set; } = code;
        /// <summary>
        /// Heigth
        /// </summary>
        public int Height { get; private set; } = height;
        /// <summary>
        /// Width
        /// </summary>
        public int Width { get; private set; } = width;
        /// <summary>
        /// Data length
        /// </summary>
        [JsonIgnore]
        public int Length { get => Height * Width * 4; }
        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; private set; } = size;
        /// <summary>
        /// Resolution
        /// </summary>
        [JsonIgnore]
        public string Resolution { get => $"{Width}x{Height}"; }
        /// <summary>
        /// Color
        /// </summary>
        public ColorType Color { get; private set; } = color;
        /// <summary>
        /// Image infor
        /// </summary>
        public SKImageInfo Infor = new SKImageInfo((width % 8) == 0 ? width : (((width / 8) + 1) * 8), height);
    }
}