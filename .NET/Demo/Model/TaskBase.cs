using Demo_Common.Enum;

namespace Demo_WPF.Model
{
    /// <summary>
    /// Task base
    /// </summary>
    public class TaskBase : ModelBase
    {
        protected string[] id = [];
        protected bool r = false;
        protected bool g = false;
        protected bool b = false;
        protected int token = 0;
        protected Pattern pattern = Pattern.UpdateDisplay;
        protected PageIndex page = PageIndex.P0;
        protected string path = string.Empty;

        /// <summary>
        /// Tag ID
        /// </summary>
        public string[] ID { get => id; set { id = value; NotifyPropertyChanged(nameof(ID)); } }
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
        public PageIndex PageIndex { get => page; set { page = value; NotifyPropertyChanged(nameof(PageIndex)); } }
        /// <summary>
        /// Path
        /// </summary>
        public string Path { get => path; set { path = value; NotifyPropertyChanged(nameof(Path)); } }
    }
}
