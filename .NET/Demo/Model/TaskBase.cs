using Demo_Common.Enum;

namespace Demo_WPF.Model
{
    /// <summary>
    /// Task base
    /// </summary>
    internal class TaskBase : ModelBase
    {
        protected string id = string.Empty;
        protected bool r = false;
        protected bool g = false;
        protected bool b = false;
        protected int times = 0;
        protected int token = 0;
        protected Pattern pattern = Pattern.UpdateDisplay;
        protected PageIndex page = PageIndex.P0;
        protected string path = string.Empty;
        protected byte[] data = [];

        /// <summary>
        /// Tag ID
        /// </summary>
        public string ID { get => id; set { id = value; NotifyPropertyChanged(nameof(ID)); } }
        /// <summary>
        /// Red
        /// </summary>
        public bool R { get => r; set { r = value; NotifyPropertyChanged(nameof(R)); } }
        /// <summary>
        /// Green
        /// </summary>
        public bool G { get => g; set { g = value; NotifyPropertyChanged(nameof(G)); } }
        /// <summary>
        /// Blue
        /// </summary>
        public bool B { get => b; set { b = value; NotifyPropertyChanged(nameof(B)); } }
        /// <summary>
        /// Times
        /// </summary>
        public int Times { get => times; set { times = value; NotifyPropertyChanged(nameof(Times)); } }
        /// <summary>
        /// Token
        /// </summary>
        public int Token { get => token; set { token = value; NotifyPropertyChanged(nameof(Token)); } }
        /// <summary>
        /// Pattern
        /// </summary>
        public Pattern Pattern { get => pattern; set { pattern = value; NotifyPropertyChanged(nameof(Pattern)); } }
        /// <summary>
        /// Page
        /// </summary>
        public PageIndex Page { get => page; set { page = value; NotifyPropertyChanged(nameof(Page)); } }
        /// <summary>
        /// Path
        /// </summary>
        public string Path { get => path; set { path = value; NotifyPropertyChanged(nameof(Path)); } }
        /// <summary>
        /// Data
        /// </summary>
        public byte[] Data { get => data; set { data = value; NotifyPropertyChanged(nameof(Data)); } }
    }
}
