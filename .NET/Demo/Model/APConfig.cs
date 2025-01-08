namespace Demo_WPF.Model
{
    /// <summary>
    /// Connection configuration
    /// </summary>
    public class APConfig : ModelBase
    {
        public string userName = string.Empty;
        public string password = string.Empty;
        public int port = 9071;

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get => userName; set { userName = value; NotifyPropertyChanged(nameof(UserName)); } } 
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get => password; set { password = value; NotifyPropertyChanged(nameof(Password)); } } 
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get => port; set { port = value; NotifyPropertyChanged(nameof(Port)); } } 
    }
}