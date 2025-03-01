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
        /// Default constructor
        /// </summary>
        public ApConfigViewModel() : base()
        {
            Config = new ApConfigModel();
            CmdConfig = new MyAsyncCommand(DoConfig, CanConfig);
        }

        /// <summary>
        /// Can config
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanConfig(object obj) => true;

        /// <summary>
        /// Do config
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task DoConfig(object arg)
        {
            try
            {
                var result = await SendService.Instance.Send(0x01, "configure", config as ApConfig);
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
        /// Set config
        /// </summary>
        /// <param name="id"></param>
        /// <param name="infor"></param>
        public void SetConfig(string id, ApInfor infor)
        {
            try
            {
                Config.Id = id;
                Config.AliasM = infor.Alias;
                Config.ServerM = infor.Server;
                Config.UserName = infor.ConnParam.Length == 2 ? infor.ConnParam[0] : string.Empty;
                Config.Password = infor.ConnParam.Length == 2 ? infor.ConnParam[1] : string.Empty;
                Config.EncryptM = infor.Encrypt;
                Config.AutoIPM = infor.AutoIP;
                Config.LocalIPM = infor.LocalIP;
                Config.SubnetM = infor.Subnet;
                Config.GatewayM = infor.Gateway;
                Config.HeartbeatM = infor.Heartbeat;
            }
            catch (Exception ex)
            {
                MsgHelper.Error("Init_Config_Error:" + ex.Message);
            }
        }
    }
}
