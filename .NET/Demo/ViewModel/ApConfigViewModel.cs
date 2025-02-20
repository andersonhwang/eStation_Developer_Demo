using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Service;
using Demo_WPF.Helper;
using Demo_WPF.Model;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class ApConfigViewModel : DialogViewModelBase
    {
        private ApConfigModel config = new();
        /// <summary>
        /// AP config
        /// </summary>
        public ApConfigModel Config { get { return config; } set { config = value; NotifyPropertyChanged(nameof(Config)); } }
        /// <summary>
        /// Command - Config AP
        /// </summary>
        public ICommand CmdConfig { get; set; }
        /// <summary>
        /// Command - Cancel
        /// </summary>
        public ICommand CmdCancel { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApConfigViewModel()
        {
            Config = new ApConfigModel();

            CmdConfig = new MyAsyncCommand(DoConfig, CanConfig);
            CmdCancel = new MyCommand(DoCancel, CanCancel);
        }

        /// <summary>
        /// Can config
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanConfig(object obj) => true;

        /// <summary>
        /// Can cancel
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanCancel(object obj) => true;

        /// <summary>
        /// Do config
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task DoConfig(object arg)
        {
            try
            {
                var result = await SendService.Instance.Send(config.Id, 0x01, $"/estation/{config.Id}/config", config as ApConfig);
                if (result != SendResult.Success)
                {
                    MsgHelper.Error("Config failed:" + result);
                    return;
                }
            }
            catch (Exception ex)
            {
                MsgHelper.Error(ex.Message);
            }
            finally
            {
                DialogResult = true;
            }
        }

        /// <summary>
        /// Do cancel
        /// </summary>
        /// <param name="obj"></param>
        private void DoCancel(object obj)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Set config
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ap"></param>
        public void SetConfig(string id, Ap ap)
        {
            try
            {
                Config.Id = id;
                Config.AliasM = ap.Infor.Alias;
                Config.ServerM = ap.Infor.Server;
                Config.UserName = ap.Infor.ConnParam.Length == 2 ? ap.Infor.ConnParam[0] : string.Empty;
                Config.Password = ap.Infor.ConnParam.Length == 2 ? ap.Infor.ConnParam[1] : string.Empty;
                Config.EncryptM = ap.Infor.Encrypt;
                Config.AutoIPM = ap.Infor.AutoIP;
                Config.LocalIPM = ap.Infor.LocalIP;
                Config.SubnetM = ap.Infor.Subnet;
                Config.GatewayM = ap.Infor.Gateway;
                Config.HeartbeatM = ap.Infor.Heartbeat;
            }
            catch (Exception ex)
            {
                MsgHelper.Error("Init_Config_Error:" + ex.Message);
            }
        }
    }
}
