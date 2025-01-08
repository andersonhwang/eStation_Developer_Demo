using Demo_Common.Entity;
using MessagePack;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Serilog;
using System.Collections.Concurrent;
using System.Text.Json;
using ClientStatus = Demo_Common.Enum.ClientStatus;

namespace Demo_Common.Service
{
    /// <summary>
    /// MQTT service
    /// </summary>
    public class MQTTService
    {
        /// <summary>
        /// Connection infor
        /// </summary>
        private ServerInfor _serverInfor;
        /// <summary>
        /// Mqtt server
        /// </summary>
        private MqttServer _server;
        /// <summary>
        /// Mqtt factory
        /// </summary>
        private readonly MqttServerFactory _factory = new();
        /// <summary>
        /// The service
        /// </summary>
        readonly static MQTTService _service = new();
        /// <summary>
        /// Clients list
        /// </summary>
        readonly List<ClientInfor> Clients = [];
        /// <summary>
        /// Receive queue
        /// </summary>
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
        /// eStation heartbeat handler
        /// </summary>
        public Action<eStationHeartbeat>? ActHeartbeat;
        /// <summary>
        /// Instance of MQTT service
        /// </summary>
        public static MQTTService Instance => _service;

        #region Public Methods
        /// <summary>
        /// Init MQTT service
        /// </summary>
        /// <param name="serverInfor">Server infor</param>
        /// <param name="actInfor">eStation infor handler</param>
        /// <param name="actMessage">eStation message handler</param>
        /// <param name="actResult">Task result handler</param>
        /// <param name="actHeartbeat">eStation heartbeat handler</param>
        /// <returns>The task</returns>
        public async Task Init(
            ServerInfor serverInfor,
            Action<eStationInfor> actInfor, Action<eStationMessage> actMessage,
            Action<TaskResult> actResult, Action<eStationHeartbeat> actHeartbeat)
        {
            ActApInfor = actInfor;
            ActApMessage = actMessage;
            ActTaskResult = actResult;
            ActHeartbeat = actHeartbeat;
            _serverInfor = serverInfor;

            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(_serverInfor.Port)
                .Build();
            _server = _factory.CreateMqttServer(options);
            _server.ValidatingConnectionAsync += Server_ValidatingConnectAsync;
            _server.InterceptingPublishAsync += Server_InterceptingPublishAsync;
            _server.ClientDisconnectedAsync += Server_ClientDisconnectedAsync;
            await _server.StartAsync();

            var task = new Task(async () =>
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
                        var payload = data.ApplicationMessage.Payload;
                        if (topic.Count(x => x.Equals('/')) != 3)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            continue;   // Invalid topic
                        }

                        var items = topic.Split('/');
                        var debug = string.Empty;
                        switch (items[3].ToLower())
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
                                var heartbeat = MessagePackSerializer.Deserialize<eStationHeartbeat>(payload);
                                client.HeartbeatTime = DateTime.Now;
                                debug = JsonSerializer.Serialize(heartbeat);
                                break;
                            case "message":
                                var message = MessagePackSerializer.Deserialize<eStationMessage>(payload);
                                ActApMessage?.Invoke(message);
                                client.HeartbeatTime = DateTime.Now;
                                debug = JsonSerializer.Serialize(message);
                                break;
                            case "infor":
                                var infor = MessagePackSerializer.Deserialize<eStationInfor>(payload);
                                ActApInfor?.Invoke(infor);
                                client.ReceiveTime = DateTime.Now;
                                client.Infor = infor;
                                client.MAC = infor.MAC;
                                client.Firmware = infor.ApVersion;
                                debug = JsonSerializer.Serialize(infor);
                                break;
                        }

                        //Log.Debug($"{topic} recv: {debug}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Process_Message_Error");
                    }
                }
            });
            task.RunSynchronously();
        }

        /// <summary>
        /// Publish application messages
        /// </summary>
        /// <param name="clientID">Client ID, for display</param>
        /// <param name="message">MQTT application message</param>
        public async Task PublishMessage<T>(string clientID, string topic, T t)
        {
            try
            {
                var data = MessagePackSerializer.Serialize(t);
                var mqtt = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(data)
                    .Build();

                // Now inject the new message at the broker.
                await _server.InjectApplicationMessage(
                    new InjectedMqttApplicationMessage(mqtt)
                    {
                        SenderClientId = clientID
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Server validating connect
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Task Server_ValidatingConnectAsync(ValidatingConnectionEventArgs args)
        {
            args.ReasonCode = args.UserName == _serverInfor.UserName && args.Password == _serverInfor.Password
            ? MqttConnectReasonCode.Success
            : MqttConnectReasonCode.UnspecifiedError;

            if (args.ReasonCode == MqttConnectReasonCode.Success)
            {
                _server.SubscribeAsync(args.ClientId,
            [
                new() { Topic = $"/estation/{args.ClientId}/infor", QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce, NoLocal = false },
                new() { Topic = $"/estation/{args.ClientId}/message", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce, NoLocal = false },
                new() { Topic = $"/estation/{args.ClientId}/heartbeat", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce, NoLocal = false },
                new() { Topic = $"/estation/{args.ClientId}/result", QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce, NoLocal = false },
            ]);
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
            }

            Log.Warning("Client_Valid_OK:" + args.ClientId + "," + args.RemoteEndPoint);
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
                Log.Warning("Client_Disconnect:" + arg.ClientId + "," + arg.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Server_InterceptingPublishAsync");
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}