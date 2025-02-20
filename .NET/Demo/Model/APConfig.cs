using Demo_Common.Entity;
using System.ComponentModel;

namespace Demo_WPF.Model
{
    /// <summary>
    /// Connection configuration
    /// </summary>
    public class ApConfigModel : ApConfig, INotifyPropertyChanged
    {
        /// <summary>
        /// AP ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Alias
        /// </summary>
        public string AliasM { get => Alias; set { Alias = value; NotifyPropertyChanged(nameof(AliasM)); } }
        /// <summary>
        /// Server address
        /// </summary>
        public string ServerM { get => Server; set { Server = value; NotifyPropertyChanged(nameof(ServerM)); } }
        /// <summary>
        /// User name
        /// </summary>
        public string UserName
        {
            get => ConnParam.Length == 2 ? ConnParam[0] : string.Empty;
            set { ConnParam[0] = value; NotifyPropertyChanged(nameof(UserName)); }
        }
        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get => ConnParam.Length == 2 ? ConnParam[1] : string.Empty;
            set { ConnParam[1] = value; NotifyPropertyChanged(nameof(Password)); }
        }
        /// <summary>
        /// Use TLS12
        /// </summary>
        public bool EncryptM { get => Encrypt; set { Encrypt = value; NotifyPropertyChanged(nameof(EncryptM)); } }
        /// <summary>
        /// Auto IP
        /// </summary>
        public bool AutoIPM { get => AutoIP; set { AutoIP = value; NotifyPropertyChanged(nameof(AutoIPM)); } }
        /// <summary>
        /// Local IP
        /// </summary>
        public string LocalIPM { get => LocalIP; set { LocalIP = value; NotifyPropertyChanged(nameof(LocalIPM)); } }
        /// <summary>
        /// Subnet mask
        /// </summary>
        public string SubnetM { get => Subnet; set { Subnet = value; NotifyPropertyChanged(nameof(SubnetM)); } }
        /// <summary>
        /// Gateway
        /// </summary>
        public string GatewayM { get => Gateway; set { Gateway = value; NotifyPropertyChanged(nameof(GatewayM)); } }
        /// <summary>
        /// Heartbeat speend, >= 15 seconds
        /// </summary>
        public int HeartbeatM { get => Heartbeat; set { Heartbeat = value; NotifyPropertyChanged(nameof(HeartbeatM)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}