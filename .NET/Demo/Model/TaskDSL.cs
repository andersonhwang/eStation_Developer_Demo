namespace Demo_WPF.Model
{
    /// <summary>
    /// Task DSL
    /// </summary>
    internal class TaskDSL : TaskBase
    {
        private int period = 3600;
        private int interval = 1000;
        private int duration = 50;

        /// <summary>
        /// Period(s), 60~3600, Default 3600
        /// </summary>
        public int Period { get => period; set { period = value; NotifyPropertyChanged(nameof(Period)); } }
        /// <summary>
        /// Interval(ms), 100~10000, Default 1000
        /// </summary>
        public int Interval { get => interval; set { interval = value; NotifyPropertyChanged(nameof(Interval)); } }
        /// <summary>
        /// Duration(ms), 50~100, Default 50
        /// </summary>
        public int Duration { get => duration; set { duration = value; NotifyPropertyChanged(nameof(Duration)); } }
    }
}
