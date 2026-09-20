using Demo_Common.Helper;

namespace Demo_WPF.Model
{
    /// <summary>
    /// Web host information
    /// </summary>
    internal class WebInfo : ModelBase
    {
        private string ip = string.Empty;
        private string[] tags = [];
        /// <summary>
        /// IP address
        /// </summary>
        public string IP { get => ip; set { ip = value; NotifyPropertyChanged(nameof(IP)); } }
        /// <summary>
        /// Tags
        /// </summary>
        public string[] Tags
        {
            get => tags;
            set
            {
                tags = value;
                if (tags.Length == 0) Code = string.Empty;
                else
                {
                    var tagType = TagHelper.GetTagType(tags[0]);
                    Code = tagType.Code;
                }
                NotifyPropertyChanged(nameof(Tags));
                NotifyPropertyChanged(nameof(Count));
                NotifyPropertyChanged(nameof(Code));
            }
        }
        public int Count { get => tags.Length; }
        /// <summary>
        /// Type code
        /// </summary>
        public string Code { get; private set; } = string.Empty;
    }

    /// <summary>
    /// Network information
    /// </summary>
    internal class NetworkInfo : ModelBase
    {
        private string adapter = string.Empty;
        private string ip = string.Empty;
        /// <summary>
        /// Adapter name
        /// </summary>
        public string Adapter { get => adapter; set { adapter = value; NotifyPropertyChanged(nameof(Adapter)); NotifyPropertyChanged(nameof(Display)); } }
        /// <summary>
        /// Display string
        /// </summary>
        public string Display => $"{Adapter}({IP})";
        /// <summary>
        /// IP address
        /// </summary>
        public string IP { get => ip; set { ip = value; NotifyPropertyChanged(nameof(IP)); } }
    }
}
