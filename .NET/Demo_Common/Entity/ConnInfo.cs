namespace Demo_Common.Entity
{
    /// <summary>
    /// Connection infor
    /// </summary>
    public class ConnInfo
    {
        /// <summary>
        /// TCP port, default is 9071
        /// </summary>
        public int Port = 9071;
        /// <summary>
        /// User name, default is test
        /// </summary>
        public string UserName = "test";
        /// <summary>
        /// Password, default is 123456
        /// </summary>
        public string Password = "123456";
        /// <summary>
        /// Encrypt with Tsl12
        /// </summary>
        public bool Encrypt = false;
        /// <summary>
        /// Certificate path
        /// </summary>
        public string Certificate = string.Empty;
        /// <summary>
        /// Certificate key
        /// </summary>
        public string CertificateKey = string.Empty;
    }
}
