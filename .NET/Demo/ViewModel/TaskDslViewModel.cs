using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Service;
using Demo_WPF.Helper;
using Demo_WPF.Model;
using Microsoft.Win32;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class TaskDslViewModel : DialogViewModelBase
    {
        private TaskDSL dsl = new();
        /// <summary>
        /// Task esl
        /// </summary>
        public TaskDSL Dsl { get => dsl; set { dsl = value; NotifyPropertyChanged(nameof(Dsl)); } }
        /// <summary>
        /// Command - browse
        /// </summary>
        public ICommand CmdBrowse { get; set; }
        /// <summary>
        /// Command - publish
        /// </summary>
        public ICommand CmdPublish { get; set; }
        /// <summary>
        /// Send handler
        /// </summary>
        public Action<string[]> SendHandler;

        /// <summary>
        /// Constructor 
        /// </summary>
        public TaskDslViewModel()
        {
            CmdPublish = new MyAsyncCommand(DoPublish, CanPublish);
            CmdBrowse = new MyCommand(DoBrowse, CanBrowse);
        }

        /// <summary>
        /// Set tags
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="tags">Tags</param>
        /// <param name="action">Action - send handler</param>
        public void SetTags(int token, string[] tags, Action<string[]> action)
        {
            Dsl.ID = tags;
            Dsl.Token = token;
            SendHandler = action;
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
                    RestoreDirectory = true,
                    DefaultDirectory = Path.GetDirectoryName(Environment.ProcessPath),
                    DefaultExt = ".bin",
                    Filter = "Binary|*.bin"
                };

                bool? result = dialog.ShowDialog();
                if (result != true) return;

                Dsl.Path = dialog.FileName;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Load_Firmware_Error");
            }
        }

        /// <summary>
        /// Do publish
        /// </summary>
        /// <param name="arg">Parameter</param>
        /// <returns>The task</returns>
        private async Task DoPublish(object arg)
        {
            var list = new List<DSLEntity>();
            var bytes = File.ReadAllBytes(Dsl.Path);
            foreach (var id in Dsl.ID)
            {
                list.Add(new DSLEntity { 
                    TagID = id, 
                    HexData = bytes,
                    Token = Dsl.Token,
                    R = Dsl.R,
                    G = Dsl.G,
                    B = Dsl.B,
                    Period = Dsl.Period,
                    Interval = Dsl.Interval,
                    Duration = Dsl.Duration
                });
            }
            var result = await SendService.Instance.Send(0x04, "taskDSL", list);
            if (result == SendResult.Success)
            {
                SendHandler?.Invoke(Dsl.ID);
            }
            else
            {
                MsgHelper.Error("Publish_Task_Error:" + result.ToString());
            }
            DialogResult = true;
        }

        /// <summary>
        /// Can browse
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>True</returns>
        private bool CanBrowse(object arg) => true;

        /// <summary>
        /// Can publish
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Result</returns>
        private bool CanPublish(object obj)
        {
            return File.Exists(Dsl.Path);
        }
    }
}
