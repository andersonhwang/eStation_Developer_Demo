using ReactiveUI;

namespace eStationDemo.Model
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
        public string UserName { get => userName; set => this.RaiseAndSetIfChanged(ref userName, value); } 
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); } 
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get => port; set => this.RaiseAndSetIfChanged(ref port, value); } 
    }
}