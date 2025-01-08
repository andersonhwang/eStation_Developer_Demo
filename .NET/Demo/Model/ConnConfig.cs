namespace Demo_WPF.Model
{
    /// <summary>
    /// Connection configuration
    /// </summary>
    public class ConnConfig : ModelBase
    {
        public string userName = string.Empty;
        public string password = string.Empty;
        public int port = 9071;
        public bool tsl12 = false;
        public string certificate = string.Empty;
        public string certificatePassword = string.Empty;

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
        /// <summary>
        /// Tsl12
        /// </summary>
        public bool Tsl12 { get => tsl12; set { tsl12 = value; NotifyPropertyChanged(nameof(Tsl12)); } }
    }
}