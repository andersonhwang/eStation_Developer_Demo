namespace Demo_WPF.Model
{
    public class TaskESL : TaskBase
    {
        protected int times = 60;
        private string oldKey = string.Empty;
        private string newKey = string.Empty;

        /// <summary>
        /// Times
        /// </summary>
        public int Times { get => times; set { times = value; NotifyPropertyChanged(nameof(Times)); } }
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