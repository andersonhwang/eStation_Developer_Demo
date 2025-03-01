using Demo_Common.Entity;
using Demo_Common.Enum;
using System.ComponentModel;

namespace Demo_WPF.Model
{
    public class ApOTA : OTAData, INotifyPropertyChanged
    {
        private bool isAP = true;
        private string tagType = string.Empty;

        /// <summary>
        /// Is AP
        /// </summary>
        public bool IsAP { get => isAP; set { isAP = value; NotifyPropertyChanged(nameof(IsAP)); } }
        /// <summary>
        /// Tag type
        /// </summary>
        public string TagType { get => tagType; set { tagType = value; NotifyPropertyChanged(nameof(TagType)); } }
        /// <summary>
        /// Firmware donwload url
        /// </summary>
        public string DownloadUrlM { get => DownloadUrl; set { DownloadUrl = value; NotifyPropertyChanged(nameof(DownloadUrlM)); } }
        /// <summary>
        /// Firmware download confirm url
        /// </summary>
        public string ConfirmUrlM { get => ConfirmUrl; set { ConfirmUrl = value; NotifyPropertyChanged(nameof(ConfirmUrlM)); } } 
        /// <summary>
        /// OTA type: 1-eStation_Developer_Edition, 2-Mod_Data, 3-Mod_Heartbeat, 4-ESL, 5-DSL
        /// </summary>
        public OtaType TypeM { get => (OtaType)Type; set { Type = (int)value; NotifyPropertyChanged(nameof(TypeM)); IsAP = value == OtaType.AP; } } 
        /// <summary>
        /// Version
        /// </summary>
        public string VersionM { get => Version; set { Version = value; NotifyPropertyChanged(nameof(Version)); } } 
        /// <summary>
        /// Firmare name
        /// </summary>
        public string NameM { get => Name; set { Name = value; NotifyPropertyChanged(nameof(NameM)); } } 
        /// <summary>
        /// MD5 Check String
        /// </summary>
        public string MD5M { get => MD5; set { MD5 = value; NotifyPropertyChanged(nameof(MD5M)); } } 

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
