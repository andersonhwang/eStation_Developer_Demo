namespace Demo_WPF.Model
{
    public class TaskESL : TaskBase
    {
        protected int times = 60;
        private string currentKey = string.Empty;
        private string newKey = string.Empty;

        /// <summary>
        /// Times
        /// </summary>
        public int Times { get => times; set { times = value; NotifyPropertyChanged(nameof(Times)); } }
        /// <summary>
        /// Current key
        /// </summary>
        public string CurrentKey { get => currentKey; set { currentKey = value; NotifyPropertyChanged(nameof(CurrentKey)); } }
        /// <summary>
        /// New key
        /// </summary>
        public string NewKey { get => newKey; set { newKey = value; NotifyPropertyChanged(nameof(NewKey)); } }
    }
}