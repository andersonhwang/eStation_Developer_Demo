using Demo_Common.Entity;
using Demo_Common.Enum;
using MessagePack;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Demo_Common.Service
{
    /// <summary>
    /// Send service
    /// </summary>
    public class SendService
    {
        private readonly object _locker = new();
        private readonly ConcurrentQueue<ApData> RecvQueue = [];
        private readonly Dictionary<string, Ap> Clients = [];
        /// <summary>
        /// Current AP
        /// </summary>
        public string CurrentAP { get; private set; } = string.Empty;
        private MQTT mqtt;
        private static SendService instance = new();
        /// <summary>
        /// Send service instance
        /// </summary>
        public static SendService Instance => instance;

        public delegate void ApStatusDelegate(string id, string ip, ApStatus status);
        public event ApStatusDelegate? ApStatusHandler;
        /// <summary>
        /// AP status delegate
        /// </summary>
        /// <param name="status">AP status</param>
        public delegate void ApConfigDelegate(string id, ApConfig config);
        public event ApConfigDelegate? ApConfigHandler;
        /// <summary>
        /// AP infor delegate
        /// </summary>
        /// <param name="infor">AP information</param>
        public delegate void ApInforDelegate(string id, ApInfor infor);
        public event ApInforDelegate? ApInforHandler;
        /// <summary>
        /// AP heartbeat delegate
        /// </summary>
        /// <param name="heartbeat">AP heartbeat</param>
        public delegate void ApHeartbeatDelegate(string id, ApHeartbeat heartbeat);
        public event ApHeartbeatDelegate? ApHeartbeatHandler;
        /// <summary>
        /// Task response delegate
        /// </summary>
        /// <param name="response">Task response</param>
        public delegate void ApMessageDelegate(string id, ApMessage message);
        public event ApMessageDelegate? ApMessageHandler;
        /// <summary>
        /// Task result delegate
        /// </summary>
        /// <param name="result"></param>
        public delegate void TaskResultDelegate(string id, TaskResult result);
        public event TaskResultDelegate? TaskResultHandler;
        /// <summary>
        /// Debug request delegate
        /// </summary>
        /// <param name="request">True is from server side, false is from ap side</param>
        /// <param name="item">Debug item</param>
        public delegate void DebugDelegate(bool request, DebugItem item);
        public event DebugDelegate? DebugHandler;
        /// <summary>
        /// Select AP handler
        /// </summary>
        /// <param name="ap"></param>
        public delegate void SelectApDelegate(Ap ap);
        public event SelectApDelegate? SelectApHandler;
        /// <summary>
        /// Select tags handler
        /// </summary>
        /// <param name="tags"></param>
        public delegate void SeletcTagDelegate(string[] tags);
        public event SeletcTagDelegate? SelectTagHandler;

        /// <summary>
        /// Run send service
        /// </summary>
        /// <param name="conn">Connection information</param>
        public bool Run(ConnInfo conn)
        {
            try
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (!RecvQueue.TryDequeue(out var item))
                            {
                                await Task.Delay(200);
                                continue;
                            }
                            if(!Clients.ContainsKey(item.Id)) continue; 

                            var ap = Clients[item.Id];
                            var json = string.Empty;
                            var alias = item.TopicAlias;
                            // Previous
                            if(alias == 0)
                            {
                                if (item.Topic.EndsWith("infor")) alias = 0x80;
                                else if (item.Topic.EndsWith("message")) alias = 0x81;
                                else if (item.Topic.EndsWith("result")) alias = 0x82;
                                else if (item.Topic.EndsWith("heartbeat")) alias = 0x83;
                            }
                            switch (alias)
                            {
                                case 0x80:
                                    var infor = MessagePackSerializer.Deserialize<ApInfor>(item.Data);
                                    if (infor is null) continue;
                                    json = JsonSerializer.Serialize(infor);
                                    UpdateInfor(item.Id, infor);
                                    ApInforHandler?.Invoke(item.Id, infor);
                                    break;
                                case 0x81:
                                    var message = MessagePackSerializer.Deserialize<ApMessage>(item.Data);
                                    if (message is null) continue;
                                    json = JsonSerializer.Serialize(message);
                                    ApMessageHandler?.Invoke(item.Id, message);
                                    break;
                                case 0x82:
                                    var result = MessagePackSerializer.Deserialize<TaskResult>(item.Data);
                                    if (result is null) continue;
                                    ap.ReceiveTime = DateTime.Now;
                                    ap.SendCount = result.SendCount;
                                    ap.WaitCount = result.WaitCount;
                                    json = JsonSerializer.Serialize(result);
                                    TaskResultHandler?.Invoke(item.Id, result);
                                    break;
                                case 0x83:
                                    var heartbeat = MessagePackSerializer.Deserialize<ApHeartbeat>(item.Data);
                                    if (heartbeat is null) continue;
                                    ap.HeartbeatTime = DateTime.Now;
                                    ap.SendCount = heartbeat.SendCount;
                                    ap.WaitCount = heartbeat.WaitCount;
                                    json = JsonSerializer.Serialize(heartbeat);
                                    ApHeartbeatHandler?.Invoke(item.Id, heartbeat);
                                    break;
                                default:
                                    break;
                            }
                            DebugHandler?.Invoke(false, new DebugItem(item, json));
                            if(item.Id.Equals(CurrentAP)) SelectApHandler?.Invoke(ap);
                        }
                        catch
                        {
                            // Not important
                        }
                    }
                });

                mqtt = new MQTT(conn, ProcessApStatus, ProcessApData);
                return mqtt.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Send_Service_Error");
                return false;
            }
        }

        /// <summary>
        /// Register AP status event
        /// </summary>
        /// <param name="status"></param>
        public void Register(ApStatusDelegate status) => ApStatusHandler += status;

        /// <summary>
        /// Register AP information event
        /// </summary>
        /// <param name="infor">AP status event</param>
        public void Register(ApInforDelegate infor) => ApInforHandler += infor;

        /// <summary>
        /// Register AP heartbeat event
        /// </summary>
        /// <param name="heartbeat">AP data event</param>
        public void Register(ApHeartbeatDelegate heartbeat) => ApHeartbeatHandler += heartbeat;

        /// <summary>
        /// Register task response event
        /// </summary>
        /// <param name="response">Task response event</param>
        public void Register(ApMessageDelegate message) => ApMessageHandler += message;

        /// <summary>
        /// Register task result event
        /// </summary>
        /// <param name="result">Task result event</param>
        public void Register(TaskResultDelegate result) => TaskResultHandler += result;

        /// <summary>
        /// Register debug request event
        /// </summary>
        /// <param name="debug">Debug request event</param>
        public void Register(DebugDelegate debug) => DebugHandler += debug;

        /// <summary>
        /// Register select client event
        /// </summary>
        /// <param name="ap"></param>
        public void Register(SelectApDelegate ap) => SelectApHandler += ap;

        /// <summary>
        /// Register select tag event
        /// </summary>
        /// <param name="tag">Tags ID array</param>
        public void Register(SeletcTagDelegate tag) => SelectTagHandler += tag;

        /// <summary>
        /// AP status handler
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <param name="status">AP status</param>
        public void ProcessApStatus(string id, string ip, ApStatus status)
        {
            try
            {
                lock (_locker)
                {
                    if (!Clients.ContainsKey(id))
                    {
                        Clients.Add(id, new Ap(id, ip, status));
                    }

                    if (Clients.TryGetValue(id, out Ap? ap))
                    {
                        ap.Status = status;
                        switch (ap.Status)
                        {
                            case ApStatus.Online:
                                ap.ConnectTime = DateTime.Now;
                                break;
                            case ApStatus.Offline:
                                ap.DisconnectTime = DateTime.Now;
                                break;
                        }
                        if (id.Equals(CurrentAP)) SelectApHandler?.Invoke(ap);
                    }
                }
                ApStatusHandler?.Invoke(id, ip, status);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Process_Ap_Status_Error");
            }
        }

        /// <summary>
        /// AP data handler
        /// </summary>
        /// <param name="data">AP data</param>
        public void ProcessApData(ApData data)
        {
            RecvQueue.Enqueue(data);
            if (Clients.TryGetValue(data.Id, out Ap? ap))
            {
                ap.ReceiveTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Send data with current AP
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="alias">Topic alias</param>
        /// <param name="topic">Topic</param>
        /// <param name="t">Data object</param>
        /// <returns>Send result</returns>
        public async Task<SendResult> Send<T>(ushort alias, string topic, T t) =>
            await Send(CurrentAP, alias, topic, t);

        /// <summary>
        /// Send data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="id">AP ID</param>
        /// <param name="alias">Topic alias</param>
        /// <param name="topic">Topic</param>
        /// <param name="t">Data object</param>
        /// <returns>Send result</returns>
        public async Task<SendResult> Send<T>(string id, ushort alias, string topic, T t)
        {
            if (!Clients.TryGetValue(id, out Ap? client)) return SendResult.NotExist;
            if (client.Status != ApStatus.Online) return SendResult.Offline;

            DebugHandler?.Invoke(true, new DebugItem(id, alias, topic, JsonSerializer.Serialize(t)));
            return await mqtt.Send(alias, $"/estation/{id}/{topic}", t);
        }

        /// <summary>
        /// Select AP
        /// </summary>
        /// <param name="id">AP ID</param>
        public void SelectAp(string id)
        {
            if (!Clients.TryGetValue(id, out Ap? ap)) return;
            CurrentAP = id;
            SelectApHandler?.Invoke(ap);
        }

        /// <summary>
        /// Select tags
        /// </summary>
        /// <param name="ids">Tags ID</param>
        public void SelectTags(string[] ids)
        {
            SelectTagHandler?.Invoke(ids);
        }

        /// <summary>
        /// Update AP infor
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="infor">AP infor</param>
        private void UpdateInfor(string id, ApInfor infor)
        {
            if (Clients.TryGetValue(id, out Ap? ap))
            {
                ap.Infor = infor;
                ap.Firmware = infor.ApVersion;
                ap.Mac = infor.MAC;
                if (id.Equals(CurrentAP)) SelectApHandler?.Invoke(ap);
            }
        }

        /// <summary>
        /// Download confirm
        /// </summary>
        /// <param name="key">Firmware key</param>
        /// <param name="id">AP ID</param>
        internal void DownloadConfirm(string key, string id)
        {
            // TODO
        }
    }
}
