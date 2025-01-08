using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System.Threading.Tasks;
using eStationDemo.Model;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Text.Json;
using MessagePack;
using System.Collections.Concurrent;
using ClientStatus = Demo.Enum.ClientStatus;

namespace Demo_WPF.Service
{
    /// <summary>
    /// MQTT service
    /// </summary>
    internal class MQTTService
    {
        ConnConfig connConfig;
        /// <summary>
        /// Mqtt server
        /// </summary>
        MqttServer _server;
        /// <summary>
        /// Mqtt factory
        /// </summary>
        private readonly MqttServerFactory _factory = new();
        readonly static MQTTService _service = new();
        /// <summary>
        /// Clients list
        /// </summary>
        readonly List<ClientInfor> Clients = [];
        readonly static ConcurrentQueue<InterceptingPublishEventArgs> QueueReceive = new();
        /// <summary>
        /// eStation infor handler
        /// </summary>
        public Action<eStationInfor>? ActApInfor;
        /// <summary>
        /// eStation message handler
        /// </summary>
        public Action<eStationMessage>? ActApMessage;
        /// <summary>
        /// Task result handler
        /// </summary>
        public Action<TaskResult>? ActTaskResult;
        /// <summary>
        /// Instance of MQTT service
        /// </summary>
        public static MQTTService Instance => _service;

        /// <summary>
        /// Init MQTT service
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <param name="actInfor"></param>
        /// <param name="actMessage">eStation message handler</param>
        /// <param name="actResult">Task result handler</param>
        /// <returns>The task</returns>
        public async Task Init(ConnConfig conn, Action<eStationInfor> actInfor,
                            Action<eStationMessage> actMessage, Action<TaskResult> actResult)
        {
            ActApInfor = actInfor;
            ActApMessage = actMessage;
            ActTaskResult = actResult;
            connConfig = conn;

            var optiosn = _factory
                .CreateServerOptionsBuilder()
                .WithDefaultEndpointPort(conn.Port)
                .Build();
            _server = _factory.CreateMqttServer(optiosn);
            _server.ValidatingConnectionAsync += Server_ValidatingConnectAsync;
            _server.InterceptingPublishAsync += Server_InterceptingPublishAsync;
            _server.ClientDisconnectedAsync += Server_ClientDisconnectedAsync;
            await _server.StartAsync();

            await Task.Run(ProcessMessage);
        }

        #region Private Methods
        /// <summary>
        /// Server validating connect
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Task Server_ValidatingConnectAsync(ValidatingConnectionEventArgs args)
        {
            if (args.UserName != connConfig.UserName || args.Password != connConfig.Password)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }
            else
            {
                args.ReasonCode = MqttConnectReasonCode.Success;
            }
            var subscribe = _factory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(t => t.WithTopic($"/estation/{args.ClientId}/result"))
                .WithTopicFilter(t => t.WithTopic($"/estation/{args.ClientId}/message"))
                .WithTopicFilter(t => t.WithTopic($"/estation/{args.ClientId}/heartbeat"))
                .Build();
            _server.SubscribeAsync(args.ClientId, subscribe.TopicFilters);
            var client = Clients.FirstOrDefault(x => x.ID == args.ClientId);
            if (client is null)
            {
                Clients.Insert(0, new ClientInfor(args));
            }
            else
            {   
                client.Status = ClientStatus.Online;
                client.ConnectTime = DateTime.Now;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Intercepting publish
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task Server_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            QueueReceive.Enqueue(arg);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Client disconnect event
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task Server_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            try
            {
                var client = Clients.FirstOrDefault(x => x.ID == arg.ClientId);
                if (client is null) { return Task.CompletedTask; }
                if (client != null)
                {
                    client.DisconnectTime = DateTime.Now;
                    client.Status = ClientStatus.Offline;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Server_InterceptingPublishAsync");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Publish application messages
        /// </summary>
        /// <param name="clientID">Client ID, for display</param>
        /// <param name="message">MQTT application message</param>
        public async Task PublishMessage<T>(string clientID, string topic, T t)
        {
            var client = Clients.First(x => x.ID.Equals(clientID));
            if (client == null) return; // Client not found.
            client.SendTime = DateTime.Now;
            var message = new MqttApplicationMessage()
            {
                Topic = topic,
                PayloadSegment = MessagePackSerializer.Serialize(t),
            };
            await _server.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
            Log.Debug("Send:" + JsonSerializer.Serialize(message));
        }

        /// <summary>
        /// Process message thread
        /// </summary>
        /// <returns></returns>
        private async Task ProcessMessage()
        {
            while (true)
            {
                try
                {
                    if (QueueReceive.IsEmpty)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        continue;   // No data
                    }

                    if (!QueueReceive.TryDequeue(out var data) || (data is null))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        continue;   // Invalid message
                    }

                    ClientInfor? client = Clients.FirstOrDefault(x => x.ID == data.ClientId);
                    if (client == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        continue;   // Not register
                    }

                    var topic = data.ApplicationMessage.Topic;
                    var payload = data.ApplicationMessage.PayloadSegment;
                    if (topic.Count(x => x.Equals('/')) != 3)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        continue;   // Invalid topic
                    }

                    var items = topic.Split('/');
                    var debug = string.Empty;
                    switch (items[2].ToLower())
                    {
                        case "result":
                            var result = MessagePackSerializer.Deserialize<TaskResult>(payload);
                            ActTaskResult?.Invoke(result);

                            client.ReceiveTime = DateTime.Now;
                            client.TotalCount = result.TotalCount;
                            client.SendCount = result.SendCount;
                            debug = JsonSerializer.Serialize(result);
                            break;
                        case "heartbeat":
                            var infor = MessagePackSerializer.Deserialize<eStationInfor>(payload);
                            ActApInfor?.Invoke(infor);
                            client.HeartbeatTime = DateTime.Now;
                            client.Infor = infor;
                            client.MAC = infor.MAC;
                            client.Firmware = infor.ApVersion;
                            debug = JsonSerializer.Serialize(infor);
                            break;
                        case "message":
                            var message = MessagePackSerializer.Deserialize<eStationMessage>(payload);
                            ActApMessage?.Invoke(message);
                            client.HeartbeatTime = DateTime.Now;
                            debug = JsonSerializer.Serialize(message);
                            break;
                    }

                    Log.Information($"{topic} recv: {debug}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Process_Message_Error");
                }
            }
        }
        #endregion
    }
}