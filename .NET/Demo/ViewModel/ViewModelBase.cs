using Demo_WPF.Helper;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
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
        /// Select AP is connect
        /// </summary>
        public bool IsConnect { get => isConnect; set { isConnect = value; NotifyPropertyChanged(nameof(IsConnect)); } }

        /// <summary>
        /// Send service is run
        /// </summary>
        public bool IsRun { get => isRun; set { isRun = value; NotifyPropertyChanged(nameof(IsRun)); } }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ViewModelBase() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Notify property changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// View model with TCP port check
    /// </summary>
    public class TcpViewModelBase : ViewModelBase
    {
        private string port = "9071";
        /// <summary>
        /// AP port
        /// </summary>
        public string Port { get { return port; } set { port = value; NotifyPropertyChanged(nameof(Port)); } }

        /// <summary>
        /// Command - Check
        /// </summary>
        public ICommand CmdCheck { get; set; }

        public TcpViewModelBase()
        {
            CmdCheck = new MyCommand(DoCheck, CanCheck);
        }

        /// <summary>
        /// Check check
        /// </summary>
        /// <returns></returns>
        private bool CanCheck(object obj) => !string.IsNullOrEmpty(Port);

        /// <summary>
        /// Do check
        /// </summary>
        /// <param name="obj"></param>
        private void DoCheck(object obj)
        {
            if (CheckPort() > 0)
            {
                MsgHelper.Infor($"Port {Port} is avaliable");
            }
        }

        /// <summary>
        /// Check port
        /// </summary>
        /// <param name="port">Port number</param>
        /// <returns>Check result</returns>
        protected int CheckPort()
        {
            if (string.IsNullOrEmpty(Port))
            {
                MsgHelper.Error($"Port is mandatory");
                return -1;
            }

            if (!int.TryParse(port, out int value))
            {
                MsgHelper.Error($"Invliad port: {Port}");
                return 0;
            }

            if (value < 1000 || value > 0xFFFF)
            {
                MsgHelper.Error($"Invliad port: {value}");
                return 0;
            }

            var listener = new TcpListener(IPAddress.Any, value);
            try
            {
                listener.Start();
                listener.Stop();
                return value;
            }
            catch (SocketException)
            {
                MsgHelper.Error($"Port {value} is unavaliable");
                return -2;
            }
            finally
            {
                listener.Dispose();
            }
        }
    }

    /// <summary>
    /// Base of dialog view model
    /// </summary>
    public class DialogViewModelBase : ViewModelBase
    {
        private bool? dialogResult;
        /// <summary>
        /// Dialog result
        /// </summary>
        public bool? DialogResult
        {
            get => dialogResult;
            set { dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); }
        }

        /// <summary>
        /// Command - cancel
        /// </summary>
        public ICommand CmdCancel { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DialogViewModelBase()
        {
            CmdCancel = new MyCommand(DoCancel, CanCancel);
        }

        /// <summary>
        /// Can cancel
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected bool CanCancel(object obj) => true;

        /// <summary>
        /// Do cancel
        /// </summary>
        /// <param name="obj"></param>
        protected void DoCancel(object obj)
        {
            DialogResult = true;
        }
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
