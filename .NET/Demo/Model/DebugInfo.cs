namespace Demo_WPF.Model
{
    public class DebugInfo : ModelBase
    {
        private int index = -1;
        private bool request = true;
        private string header = string.Empty;
        private string description = string.Empty;
        /// <summary>
        /// Select index
        /// </summary>
        public int Index { get { return index; } set { index = value; NotifyPropertyChanged(nameof(Index)); } }
        /// <summary>
        /// Request
        /// </summary>
        public bool Request { get => request; set => request = value; }
        /// <summary>
        /// Header
        /// </summary>
        public string Header { get => header; set { header = value; NotifyPropertyChanged(nameof(Header)); } }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get => description; set { description = value; NotifyPropertyChanged(nameof(Description)); } }
    }
}