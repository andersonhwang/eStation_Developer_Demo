using System.ComponentModel;

namespace eStationDemo.Model
{
    /// <summary>
    /// Model base
    /// </summary>
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Notify property changed
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
