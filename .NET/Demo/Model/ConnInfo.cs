using Demo_Common.Entity;
using System.ComponentModel;
using System.IO;

namespace Demo_WPF.Model
{
    /// <summary>
    /// Connection configuration
    /// </summary>
    public class ConnInfoModel : ConnInfo, INotifyPropertyChanged
    {
        /// <summary>
        /// Port
        /// </summary>
        public int PortM { get => Port; set { Port = value; NotifyPropertyChanged(nameof(PortM)); } }
        /// <summary>
        /// User name, default is test
        /// </summary>
        public string UserNameM { get => UserName; set { UserName = value; NotifyPropertyChanged(nameof(UserNameM)); } }
        /// <summary>
        /// Password, default is 123456
        /// </summary>
        public string PasswordM { get => Password; set { Password = value; NotifyPropertyChanged(nameof(PasswordM)); } }
        /// <summary>
        /// Security, default is true
        /// </summary>
        public bool EncryptM { get => Encrypt; set { Encrypt = value; NotifyPropertyChanged(nameof(EncryptM2)); } }
        /// <summary>
        /// Security, default is true
        /// </summary>
        public bool EncryptM2 { get => !Encrypt;  }
        /// <summary>
        /// Certificate path
        /// </summary>
        public string CertificateM { get => Certificate; set { Certificate = value; NotifyPropertyChanged(nameof(CertificateNameM)); } }
        /// <summary>
        /// Certificate name
        /// </summary>
        public string CertificateNameM { get => File.Exists(Certificate) ? Path.GetFileName(Certificate) : string.Empty; }
        /// <summary>
        /// Certificate key
        /// </summary>
        public string CertificateKeyM { get => CertificateKey; set { CertificateKey = value; NotifyPropertyChanged(nameof(CertificateKeyM)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}