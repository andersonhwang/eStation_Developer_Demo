namespace Demo_WPF.Model
{
    public class TaskESL : TaskBase
    {
        protected int times = 60;

        /// <summary>
        /// Times
        /// </summary>
        public int Times { get => times; set { times = value; NotifyPropertyChanged(nameof(Times)); } }
    }
}