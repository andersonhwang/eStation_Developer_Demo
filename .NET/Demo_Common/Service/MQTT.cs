using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Helper;
using MessagePack;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Serilog;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace Demo_Common.Service
{
    internal class MQTT
    {
        private readonly Action<string, string, ApStatus> ApStatusHandler;
        private readonly Action<ApData> ApDataHandler;
        private readonly ConnInfo Conn;
        private MqttServer mqttServer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Connection infor</param>
        /// <param name="actStatus">AP status handler</param>
        /// <param name="actData">AP data handler</param>
        public MQTT(ConnInfo info, Action<string, string, ApStatus> actStatus, Action<ApData> actData)
        {
            Conn = info;
            ApStatusHandler = actStatus;
            ApDataHandler = actData;
        }

        /// <summary>
        /// Run MQTT service
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            try
            {
                var builder = new MqttServerOptionsBuilder();
                if (Conn.Encrypt)
                {
                    var cert = X509Certificate2.CreateFromPemFile(Conn.Certificate, Conn.CertificateKey);
                    Log.Information(
                        "Certificate Subject={Subject}, Issuer={Issuer}, HasPrivateKey={HasPrivateKey}, Thumbprint={Thumbprint}",
                        cert.Subject,
                        cert.Issuer,
                        cert.HasPrivateKey,
                        cert.Thumbprint);
                    builder = builder
                        .WithEncryptedEndpoint()
                        .WithEncryptedEndpointPort(Conn.Port)
                        .WithEncryptionCertificate(cert)
                        .WithEncryptionSslProtocol(SslProtocols.Tls12);
                    if (Conn.mTLS)
                    {
                        builder = builder.WithClientCertificate(ValidateClientCertificate, false);
                    }
                }
                else
                {
                    builder = builder
                        .WithDefaultEndpoint()
                        .WithDefaultEndpointPort(Conn.Port);
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

        private bool ValidateClientCertificate(
            object sender,
            X509Certificate? certificate,
            X509Chain? originalChain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (certificate == null)
            {
                Log.Warning("MQTT_MTLS_NO_CLIENT_CERTIFICATE");
                return false;
            }

            try
            {
                using var clientCertificate = new X509Certificate2(certificate);

                using var rootCa =
                    new X509Certificate2(Conn.RootCaCertificate);

                using var issuingCa =
                    new X509Certificate2(Conn.IssuingCaCertificate);

                using var chain = new X509Chain();

                chain.ChainPolicy.TrustMode =
                    X509ChainTrustMode.CustomRootTrust;

                chain.ChainPolicy.CustomTrustStore.Add(rootCa);

                chain.ChainPolicy.ExtraStore.Add(issuingCa);

                chain.ChainPolicy.RevocationMode =
                    X509RevocationMode.NoCheck;

                chain.ChainPolicy.VerificationFlags =
                    X509VerificationFlags.NoFlag;

                // Client Authentication EKU
                chain.ChainPolicy.ApplicationPolicy.Add(
                    new Oid("1.3.6.1.5.5.7.3.2"));

                var valid = chain.Build(clientCertificate);

                if (!valid)
                {
                    foreach (var status in chain.ChainStatus)
                    {
                        Log.Warning(
                            "MQTT_MTLS_CLIENT_CERT_INVALID " +
                            "Subject={Subject}, Thumbprint={Thumbprint}, " +
                            "Status={Status}, Info={Info}",
                            clientCertificate.Subject,
                            clientCertificate.Thumbprint,
                            status.Status,
                            status.StatusInformation);
                    }

                    return false;
                }

                Log.Information(
                    "MQTT_MTLS_CLIENT_CERT_OK " +
                    "Subject={Subject}, Issuer={Issuer}, Thumbprint={Thumbprint}",
                    clientCertificate.Subject,
                    clientCertificate.Issuer,
                    clientCertificate.Thumbprint);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MQTT_MTLS_CLIENT_CERT_VALIDATE_ERR");
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
            ApStatusHandler(arg.ClientId, arg.RemoteEndPoint.ToString() ?? string.Empty, ApStatus.Online);
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
            ApStatusHandler(arg.ClientId, arg.RemoteEndPoint.ToString() ?? string.Empty, ApStatus.Offline);
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
            arg.ReasonCode = arg.UserName == Conn.UserName && arg.Password == Conn.Password
                ? MqttConnectReasonCode.Success
                : MqttConnectReasonCode.NotAuthorized;

            if (arg.ReasonCode == MqttConnectReasonCode.Success)
            {
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/infor");
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/message");
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/heartbeat");
                mqttServer.SubscribeAsync(arg.ClientId, $"/estation/{arg.ClientId}/result");
                ApStatusHandler(arg.ClientId, arg.RemoteEndPoint.ToString() ?? string.Empty, ApStatus.Connecting);
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