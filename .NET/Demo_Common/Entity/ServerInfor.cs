namespace Demo_Common.Entity
{
    /// <summary>
    /// Server infor
    /// </summary>
    public class ServerInfor
    {
        /// <summary>
        /// TCP port, default is 9071
        /// </summary>
        public int Port { get; set; } = 9071;
        /// <summary>
        /// User name, default is test
        /// </summary>
        public string UserName { get; set; } = "test";
        /// <summary>
        /// Password, default is 123456
        /// </summary>
        public string Password { get; set; } = "123456";
        /// <summary>
        /// Encrypt with Tsl12
        /// </summary>
        public bool Encrypt { get; set; } = false;
        /// <summary>
        /// Certificate path
        /// </summary>
        public string Certificate { get; set; } = string.Empty;
        /// <summary>
        /// Certificate key
        /// </summary>
        public string CertificateKey { get; set; } = string.Empty;
    }
}
