using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Helper;
using MessagePack;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Serilog;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace Demo_Common.Service
{
    internal class MQTT
    {
        private readonly Action<string, ApStatus> ApStatusHandler;
        private readonly Action<ApData> ApDataHandler;
        private readonly Action<string, EndPoint> ClientHandler;
        private readonly ConnInfo Connection;
        private MqttServer mqttServer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Connection infor</param>
        /// <param name="actStatus">AP status handler</param>
        /// <param name="actData">AP data handler</param>
        public MQTT(ConnInfo info, Action<string, ApStatus> actStatus, Action<ApData> actData, Action<string, EndPoint> actClient)
        {
            Connection = info;
            ApStatusHandler = actStatus;
            ApDataHandler = actData;
            ClientHandler = actClient;
        }

        /// <summary>
        /// Run MQTT service
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            try
            {
                var builder = new MqttServerOptionsBuilder()
                        .WithDefaultEndpoint();
                if (Connection.Encrypt)
                {
                    builder = builder
                        .WithEncryptedEndpoint()
                        .WithEncryptedEndpointPort(Connection.Port)
                        .WithEncryptionCertificate(FileHelper.GetCertificate(Connection.Certificate, Connection.CertificateKey))
                        .WithEncryptionSslProtocol(SslProtocols.Tls12);
                }
                else
                {
                    builder = builder
                        .WithDefaultEndpointPort(Connection.Port);
                }
                var options = builder.Build();
                mqttServer = new MqttServerFactory().CreateMqttServer(options);
                mqttServer.ClientConnectedAsync += ClientConnectedAsync;
                mqttServer.ClientDisconnectedAsync += ClientDisconnectedAsync;
                mqttServer.ValidatingConnectionAsync += ValidatingConnectionAsync;
                mqttServer.InterceptingPublishAsync += InterceptingPublishAsync;
                mqttServer.StartAsync();
                Log.Information("MQTT_RUN_OK");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MQTT_RUN_ERR");
                return false;
            }
        }

        /// <summary>
        /// Client connected
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            ApStatusHandler(arg.ClientId, ApStatus.Online);
            ClientHandler(arg.ClientId, arg.RemoteEndPoint);
            Log.Information($"{arg.ClientId}({arg.RemoteEndPoint}) connected");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Client disconnected
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>The task</returns>
        private Task ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            ApStatusHandler(arg.ClientId, ApStatus.Offline);
            Log.Warning($"{arg.ClientId}({arg.RemoteEndPoint}) disconnected");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Validating client connection
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>The task</returns>
        private Task ValidatingConnectionAsync(ValidatingConnectionEventArgs arg)
        {
            arg.ReasonCode = arg.UserName == Connection.UserName && arg.Password == Connection.Password
                ? MqttConnectReasonCode.Success
                : MqttConnectReasonCode.UnspecifiedError;

            if (arg.ReasonCode == MqttConnectReasonCode.Success)
            {
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/infor");
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/message");
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/heartbeat");
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/result");
                ApStatusHandler(arg.ClientId, ApStatus.Connecting);
            }

            Log.Information($"{arg.ClientId}({arg.RemoteEndPoint}) validating connection:{arg.ReasonCode}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Intercepting publish
        /// </summary>
        /// <param name="arg">Message</param>
        /// <returns>The task</returns>
        private Task InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            ApDataHandler(new ApData
            {
                Id = arg.ClientId,
                Topic = arg.ApplicationMessage.Topic,
                TopicAlias = arg.ApplicationMessage.TopicAlias,
                Data = arg.ApplicationMessage.Payload
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send data to client
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="alias">Topic alias</param>
        /// <param name="topic">Topic</param>
        /// <param name="t">Data object</param>
        /// <returns>Result</returns>
        public async Task<SendResult> Send<T>(ushort alias, string topic, T t)
        {
            try
            {
                var mqtt = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithTopicAlias(alias)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                    .WithPayload(MessagePackSerializer.Serialize(t))
                    .Build();
                await mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(mqtt));
                return SendResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MQTT_SEND_ERR");
                return SendResult.Error;
            }
        }
    }
}