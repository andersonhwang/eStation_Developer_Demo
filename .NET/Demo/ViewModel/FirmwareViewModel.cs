using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Helper;
using Demo_Common.Service;
using Demo_WPF.Helper;
using Demo_WPF.Model;
using Microsoft.Win32;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class FirmwareViewModel : DialogViewModelBase
    {
        private ApOTA data = new();

        /// <summary>
        /// OTA data
        /// </summary>
        public ApOTA Data { get => data; set { data = value; NotifyPropertyChanged(nameof(Data)); } }
        /// <summary>
        /// Tag types
        /// </summary>
        public List<TagType> TagTypes { get; private set; }
        /// <summary>
        /// Command - Browse
        /// </summary>
        public ICommand CmdBrowse { get; set; }
        /// <summary>
        /// Command - OTA
        /// </summary>
        public ICommand CmdOTA { get; set; }

        public FirmwareViewModel() : base()
        {
            CmdBrowse = new MyCommand(DoBrowse, CanBrowse);
            CmdOTA = new MyAsyncCommand(DoOTA, CanOTA);

            TagTypes = TagHelper.TagTypes;
        }

        /// <summary>
        /// Can browse
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanBrowse(object arg) => true;

        /// <summary>
        /// Can OTA
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanOTA(object arg)
        {
            if (string.IsNullOrEmpty(Data.NameM)) return false;
            if (string.IsNullOrEmpty(Data.VersionM)) return false;

            return true;
        }

        /// <summary>
        /// Do browse
        /// </summary>
        /// <param name="obj"></param>
        private void DoBrowse(object obj)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Multiselect = false,
                    RestoreDirectory = true,
                    DefaultDirectory = Path.GetDirectoryName(Environment.ProcessPath),
                    DefaultExt = Data.Type switch
                    {
                        1 => ".tar",
                        2 => ".bin",
                        3 => ".bin",
                        4 => ".bin",
                        5 => ".bin",
                        _ => ".tar"
                    },
                    Filter = "Firmware|*.tar|Binary|*.bin"
                };

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    Data.NameM = Data.IsAP ? Path.GetFileName(dialog.FileName) : Data.TagType;
                    Data.MD5M = FileHelper.GetBytesMd5(File.ReadAllBytes(dialog.FileName));

                    WebService.Instance.AddItem(Data.NameM, dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Load_Firmware_Error");
            }
        }

        /// <summary>
        /// Do OTA
        /// </summary>
        /// <param name="obj"></param>
        private async Task DoOTA(object obj)
        {
            try
            {
                //WebService.Instance.AddItem(Path.GetFileNameWithoutExtension(Data.Name), Data.Name);
                var ip = WebService.Instance.IP;
                var ap = SendService.Instance.CurrentAP;
                var ota = new OTAData()
                {
                    ConfirmUrl = $"http://{ip}/confirm/{ap}/{Data.NameM}?code=1&flag=A",
                    DownloadUrl = $"http://{ip}/ota/{Data.NameM}?code=2&flag=B",
                    MD5 = Data.MD5,
                    Name = Data.Name,
                    Type = Data.Type,
                    Version = Data.Version
                };
                var result = await SendService.Instance.Send(0x05, "firmware", ota);
                if (result != SendResult.Success)
                {
                    MsgHelper.Error("OTA_ERR:" + result);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "OTA_Error");
            }
            finally
            {
                DialogResult = true;
            }
        }
    }
}
