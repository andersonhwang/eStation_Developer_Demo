using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Helper;
using Demo_Common.Service;
using Demo_WPF.Model;
using Demo_WPF.View;
using Serilog;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Demo_WPF.ViewModel
{
    internal class TagListViewModel : ViewModelBase
    {
        private readonly object _locker = new();
        private readonly string TagListPath = "TagList.txt";
        private Random Random;
        private TagHeader header = new();
        private int _count = 0;
        private int token = 0;
        private bool isConnect = false;
        private ObservableCollection<Tag> tags = [];
        private ObservableCollection<ApModel> aps = [];
        /// <summary>
        /// Tags count
        /// </summary>
        public int Count { get => _count; set { _count = value; NotifyPropertyChanged(nameof(HeaderText)); } }
        /// <summary>
        /// Header text
        /// </summary>
        public string HeaderText => $"Tags List ({_count})";
        /// <summary>
        /// Token
        /// </summary>
        public int Token { get { if (++token >= 0xFFFF) token = 0; return token; } }
        /// <summary>
        /// Header
        /// </summary>
        public TagHeader Header { get => header; set { header = value; NotifyPropertyChanged(nameof(Header)); } }
        /// <summary>
        /// Is connect
        /// </summary>
        public bool IsConnect { get => isConnect; set { isConnect = value; NotifyPropertyChanged(nameof(IsConnect)); } }
        /// <summary>
        /// Tags
        /// </summary>
        public ObservableCollection<Tag> Tags { get => tags; set { tags = value; NotifyPropertyChanged(nameof(Tags)); } }
        /// <summary>
        /// AP list
        /// </summary>
        public ObservableCollection<ApModel> APs { get => aps; set { aps = value; NotifyPropertyChanged(nameof(APs)); } }

        /// <summary>
        /// Command - menu
        /// </summary>
        public ICommand CmdMenu { get; private set; }
        /// <summary>
        /// Command - select AP
        /// </summary>
        public ICommand CmdSelectAP { get; private set; }
        /// <summary>
        /// Command - select tag
        /// </summary>
        public ICommand CmdSelectTag { get; private set; }
        /// <summary>
        /// Command - select all tags
        /// </summary>
        public ICommand CmdSelectAllTags { get; private set; }
        /// <summary>
        /// Command - send
        /// </summary>
        public ICommand CmdSend { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TagListViewModel()
        {
            Random = new Random(DateTime.Now.Millisecond);
            LoadTags();

            CmdMenu = new MyCommand(DoMemu, CanMenu);
            CmdSelectAP = new MyCommand(DoSelectAP, CanSelectAP);
            CmdSelectTag = new MyCommand(DoSelectTag, CanSelectTag);
            CmdSelectAllTags = new MyCommand(DoSelectAllTags, CanSelectAllTags);
            CmdSend = new MyCommand(DoSend, CanSend);

            SendService.Instance.Register(ApStatusHandler);
            SendService.Instance.Register(ApHeartbeatHandler);
            SendService.Instance.Register(TaskResultHandler);
        }

        /// <summary>
        /// AP status handler
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="ip">AP IP</param>
        /// <param name="status">AP status</param>
        private void ApStatusHandler(string id, string ip, ApStatus status)
        {
            lock (_locker)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var ap = APs.FirstOrDefault(x => x.IDM == id);
                    if (ap == null)
                    {
                        ap = new ApModel(id, ip, status);
                        APs.Add(ap);
                    }
                    else
                    {
                        ap.StatusM = status;
                    }
                });
            }
        }

        /// <summary>
        /// AP heartbeat handler
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="heartbeat">AP heartbeat</param>
        private void ApHeartbeatHandler(string id, ApHeartbeat heartbeat)
        {
            try
            {
                if (header.OnlyData) return; // Only data
                lock (_locker)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        bool add = false;
                        foreach (var item in heartbeat.Tags)
                        {
                            var tag = Tags.FirstOrDefault(x => x.ID == item.TagId);
                            if (tag is null)
                            {
                                if (!header.AutoRegister) continue;
                                tag = new Tag(item.TagId);
                                AddTag(tag);
                                add = true;
                            }
                            tag.HeartbeatCount++;
                            tag.LastHeartbeat = DateTime.Now;
                            tag.Battery = item.Battery;
                            tag.RfPower = item.RfPower;
                            tag.Temperature = item.Temperature;
                            tag.Status = TagStatus.Heartbeat;
                            tag.Version = item.Version;
                        }

                        if (!add) return;
                        var builder = new StringBuilder();
                        foreach (var tag in Tags)
                        {
                            builder.AppendLine(id);
                        }
                        File.WriteAllText("TagList.txt", builder.ToString());
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AP_Heartbeat_Error");
            }
        }

        /// <summary>
        /// Task result handler
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="result">Task result</param>
        private void TaskResultHandler(string id, TaskResult result)
        {
            lock (_locker)
            {
                foreach (var item in result.Tags)
                {
                    var tag = Tags.FirstOrDefault(x => x.ID == item.TagId);
                    if (tag is null) continue;
                    tag.LastRecv = DateTime.Now;
                    tag.Status = GetTagStatus(item);
                    tag.Battery = item.Battery;
                    tag.RfPower = item.RfPower;
                    tag.Temperature = item.Temperature;
                    tag.Version = item.Version;

                    if (tag.Status == TagStatus.Error) tag.ErrorCount++;
                }
            }

            static TagStatus GetTagStatus(TagResult item)
            {
                if (item.RfPower == -256) return TagStatus.Error;                   // Failed
                if (item.Status == 0xF0) return TagStatus.InvaidKey;                // Invalid key
                if (item.Status == 0xF1) return TagStatus.DuplicateToken;           // Duplciate token
                if (item.Status == 0x01) return TagStatus.LcmdIdError;              // LCM ID error
                if (item.Status == 0x02) return TagStatus.McuReset;                 // Failed, MCU reset
                if (item.Status == 0x03) return TagStatus.LcmdRefreshError;         // LCM refresh error
                var type = TagHelper.GetTagType(item.TagId);
                var battery = (type.Type == "39" || type.Type == "54") ? 22 : 25;   // Type 39&54 is frozen tag
                return item.Battery < battery ? TagStatus.LowPower : TagStatus.Success;
            }
        }

        /// <summary>
        /// Can menu
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanMenu(object arg) => true;

        /// <summary>
        /// Can select AP
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CanSelectAP(object arg) => APs.Count > 0;

        /// <summary>
        /// Can select tag
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CanSelectTag(object arg) => true;

        /// <summary>
        /// Can select all tags
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CanSelectAllTags(object arg)
        {
            if (Tags.Count == 0) return false;
            var first = Tags.First().ID[..2];
            return Tags.All(x => x.ID[..2].Equals(first));
        }

        /// <summary>
        /// Can send
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanSend(object arg)
        {
            var select = Tags.Where(x => x.Select).Select(x => x);
            var result = select.Any() && select.All(x => x.ID[..2].Equals(select.First().ID[..2]));
            return Header.Same = result;
        }

        /// <summary>
        /// Do menu
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DoMemu(object obj)
        {
            switch (obj.ToString())
            {
                case "E":
                    var dialog = new winTags(Tags.Select(x => x.ID).ToList());
                    dialog.ShowDialog();
                    LoadTags();
                    break;
                case "R":
                    for (var i = 0; i < Tags.Count; i++)
                    {
                        Tags[i] = new Tag(Tags[i].ID);
                    }
                    break;
                case "P":

                    break;
            }
        }

        /// <summary>
        /// Do select AP
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DoSelectAP(object obj)
        {

        }

        /// <summary>
        /// Do select tag
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DoSelectTag(object obj)
        {
            SendService.Instance.SelectTags(Tags.Where(x => x.Select).Select(x => x.ID).ToArray());
        }

        /// <summary>
        /// Select all tags
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DoSelectAllTags(object obj)
        {

        }

        /// <summary>
        /// Do send
        /// </summary>
        /// <param name="obj"></param>
        private void DoSend(object obj)
        {
            var select = Tags.Where(x => x.Select).Select(x => x.ID);
            var first = select.First();
            if (first.StartsWith("D0"))
            {
                var dsl = new winTaskDSL(Token, select.ToArray(), SendHandler);
                dsl.ShowDialog();
            }
            else
            {
                var type = TagHelper.GetTagType(first);
                var esl = new winTaskESL(Token, select.ToArray(), SendHandler);
                esl.ShowDialog();
            }
        }

        /// <summary>
        /// Load tags
        /// </summary>
        private void LoadTags()
        {
            // Create if not exist
            if (!File.Exists(TagListPath))
            {
                using var fs = File.Create(TagListPath);
                var title = Encoding.UTF8.GetBytes(string.Empty);
                fs.Write(title, 0, title.Length);
            }

            // Read
            var lines = File.ReadAllLines(TagListPath);
            lock (_locker)
            {
                Tags.Clear();
                foreach (var line in lines)
                {
                    var id = line.Trim().ToUpper();
                    if (!Regex.IsMatch(id, "^[0-9A-F]{12}$")) continue;
                    if (Tags.Any(x => x.ID.Equals(id))) continue;
                    AddTag(new Tag(id));
                }
                if (Tags.Count > 0)
                {
                    var first = Tags.First().ID[..2];
                    Header.Same = Tags.All(x => x.ID[..2].Equals(first));
                }
            }
        }

        /// <summary>
        /// Add tag
        /// </summary>
        /// <param name="id">Tag ID</param>
        private void AddTag(Tag tag)
        {
            Tags.Add(tag);
            Count = Tags.Count;
            if (Header.Same && Tags.Count > 1)
            {
                if (!Tags.First().ID[..2].Equals(tag.ID[..2]))
                {
                    Header.Same = false;
                }
            }
        }

        /// <summary>
        /// Send handler
        /// </summary>
        /// <param name="tags">Tags</param>
        private void SendHandler(string[] tags)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                foreach (var tag in tags)
                {
                    var item = Tags.FirstOrDefault(x => x.ID.Equals(tag));
                    if (item is null) continue;
                    item.LastSend = DateTime.Now;
                    item.Status = TagStatus.Sending;
                    item.SendCount++;
                }
            });
        }
    }
}
