namespace Demo_WPF.Model
{
    /// <summary>
    /// Connection configuration
    /// </summary>
    public class ESLTask : ModelBase
    {
        private bool r = false;
        private bool g = false;
        private bool b = false;
        private int token = 0;
        private int times = 0;
        private string filePath = "";
        private string base64 = string.Empty;
        /// <summary>
        /// Red
        /// </summary>
        public bool R { get => r; set => this.RaiseAndSetIfChanged(ref r, value); }
        /// <summary>
        /// Green
        /// </summary>
        public bool G { get => r; set => this.RaiseAndSetIfChanged(ref g, value); }
        /// <summary>
        /// Blue
        /// </summary>
        public bool B { get => r; set => this.RaiseAndSetIfChanged(ref b, value); }
        /// <summary>
        /// Token
        /// </summary>
        public int Token { get => token; set => this.RaiseAndSetIfChanged(ref token, value); }
        /// <summary>
        /// Times
        /// </summary>
        public int Times { get => times; set => this.RaiseAndSetIfChanged(ref times, value); }
        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get => filePath; set => this.RaiseAndSetIfChanged(ref filePath, value); }
        /// <summary>
        /// Image base64 string
        /// </summary>
        public string Base64 { get => base64; set => this.RaiseAndSetIfChanged(ref base64, value); }
    }
}