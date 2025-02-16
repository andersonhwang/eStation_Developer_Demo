namespace Demo_WPF.Model
{
    /// <summary>
    /// Connection configuration
    /// </summary>
    public class ApConfig : ModelBase
    {
        private string alias = string.Empty;
        private string serverAddress = "192.168.4.92";
        private string userName = "test";
        private string password = "123456";
        private bool encrypted = false;
        private int port = 9071;
        private bool autoIP = true;
        private string localIP = string.Empty;
        private string subnet = string.Empty;
        private string gateway = string.Empty;
        private int heartbeat = 15;
        private int speed = 15;
        private string token = string.Empty;

        /// <summary>
        /// Alias
        /// </summary>
        public string Alias { get => alias; set { alias = value; NotifyPropertyChanged(nameof(Alias)); } }
        /// <summary>
        /// Server address
        /// </summary>
        public string ServerAddress { get => serverAddress; set { serverAddress = value; NotifyPropertyChanged(nameof(ServerAddress)); } }
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get => userName; set { userName = value; NotifyPropertyChanged(nameof(UserName)); } }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get => password; set { password = value; NotifyPropertyChanged(nameof(Password)); } }
        /// <summary>
        /// Use TLS12
        /// </summary>
        public bool Encrypted { get => encrypted; set { encrypted = value; NotifyPropertyChanged(nameof(Encrypted)); } }
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get => port; set { port = value; NotifyPropertyChanged(nameof(Port)); } }
        /// <summary>
        /// Auto IP
        /// </summary>
        public bool AutoIP { get => autoIP; set { autoIP = value; NotifyPropertyChanged(nameof(AutoIP)); } }
        /// <summary>
        /// Local IP
        /// </summary>
        public string LocalIP { get => localIP; set { localIP = value; NotifyPropertyChanged(nameof(LocalIP)); } }
        /// <summary>
        /// Subnet mask
        /// </summary>
        public string Subnet { get => subnet; set { subnet = value; NotifyPropertyChanged(nameof(Subnet)); } }
        /// <summary>
        /// Gateway
        /// </summary>
        public string Gateway { get => gateway; set { gateway = value; NotifyPropertyChanged(nameof(Gateway)); } }
        /// <summary>
        /// Heartbeat speend, >= 15 seconds
        /// </summary>
        public int Heartbeat { get => heartbeat; set { heartbeat = value; NotifyPropertyChanged(nameof(Heartbeat)); } }
        /// <summary>
        /// Broadcast speed, >= 2 seconds
        /// </summary>
        public int Speed { get => speed; set { speed = value; NotifyPropertyChanged(nameof(Speed)); } }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get => token; set { token = value; NotifyPropertyChanged(nameof(Token)); } }
    }
}