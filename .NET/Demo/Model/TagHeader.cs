namespace Demo_WPF.Model
{
    internal class TagHeader : ModelBase
    {
        private bool autoRegister = false;
        private bool onlyData = false;
        private bool same = false;
        private bool select = false;
        private string ap = string.Empty;

        /// <summary>
        /// Auto register
        /// </summary>
        public bool AutoRegister { get => autoRegister; set { autoRegister = value; NotifyPropertyChanged(nameof(AutoRegister)); } }
        /// <summary>
        /// Only data
        /// </summary>
        public bool OnlyData { get => onlyData; set { onlyData = value; NotifyPropertyChanged(nameof(OnlyData)); } }
        /// <summary>
        /// Same
        /// </summary>
        public bool Same { get => same; set { same = value; NotifyPropertyChanged(nameof(Same)); } }
        /// <summary>
        /// Select all
        /// </summary>
        public bool Select { get => select; set { select = value; NotifyPropertyChanged(nameof(Select)); } }
        /// <summary>
        /// AP
        /// </summary>
        public string AP { get => ap; set { ap = value; NotifyPropertyChanged(nameof(AP)); } }
    }
}
