using Demo_Common.Enum;
using Demo_Common.Service;
using System.ComponentModel;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    /// <summary>
    /// Base of view model
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool isConnect = false;
        private bool isRun = false;

        /// <summary>
        /// AP is connect
        /// </summary>
        public bool IsConnect
        {
            get => isConnect;
            set { isConnect = value; NotifyPropertyChanged(nameof(IsConnect)); }
        }

        /// <summary>
        /// Send service is run
        /// </summary>
        public bool IsRun
        {
            get => isRun;
            set { isRun = value; NotifyPropertyChanged(nameof(IsRun)); }
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ViewModelBase()
        {
            MQTTService.Instance.Register(ApStatusHandler);
        }

        /// <summary>
        /// Ap status handler
        /// </summary>
        /// <param name="status"></param>
        public void ApStatusHandler(ApStatus status) => IsConnect = status is ApStatus.Online or ApStatus.Working;

        /// <summary>
        /// Can send
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Yes</returns>
        public bool CanSend(object parameter) => IsConnect;

        /// <summary>
        /// Get color
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>Color</returns>
        public static int GetColor(bool r, bool g, bool b)
        {
            var data = 0;
            data += (byte)(r ? 1 : 0);                          // Bit 2
            data <<= 1;
            data += (byte)(g ? 1 : 0);                          // Bit 1
            data <<= 1;
            data += (byte)(b ? 1 : 0);                          // Bit 0
            return data;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Notify property changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Interface async command
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>The task</returns>
        Task ExecuteAsync(object parameter);
    }

    /// <summary>
    /// My command
    /// </summary>
    /// <param name="doExecute">Action execute</param>
    /// <param name="canExecute">Function can execute</param>
    public class MyCommand(Action<object> doExecute, Func<object, bool> canExecute) : ICommand
    {
        private readonly Action<object> _doExecute = doExecute;
        private readonly Func<object, bool> _canExecute = canExecute;
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Can execute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Result</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute(parameter ?? new object());
        }

        /// <summary>
        /// Do execute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public void Execute(object? parameter)
        {
            _doExecute.Invoke(parameter ?? new object());
        }
    }

    /// <summary>
    /// My async command
    /// </summary>
    /// <param name="doExecute">Action execute</param>
    /// <param name="canExecute">Function can execute</param>
    public class MyAsyncCommand(Func<object, Task> doExecute, Predicate<object> canExecute) : IAsyncCommand
    {
        private readonly Func<object, Task> _doExecute = doExecute;
        private readonly Predicate<object> _canExecute = canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public Task ExecuteAsync(object parameter)
        {
            return _doExecute(parameter);
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter ?? new object());
        }
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
