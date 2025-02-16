namespace Demo_WPF.Model
{
    internal class TaskESL : TaskBase
    {
        private string oldKey = string.Empty;
        private string newKey = string.Empty;

        /// <summary>
        /// Old key
        /// </summary>
        public string OldKey { get => oldKey; set { oldKey = value; NotifyPropertyChanged(nameof(OldKey)); } }
        /// <summary>
        /// New key
        /// </summary>
        public string NewKey { get => newKey; set { newKey = value; NotifyPropertyChanged(nameof(NewKey)); } }
    }
}
