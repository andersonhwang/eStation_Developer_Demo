using Demo_Common.Entity;
using Demo_Common.Service;
using Serilog;
using SkiaSharp;
using System;
using System.IO.Compression;
using System.Text.Json;

namespace Demo_Console
{
    /// <summary>
    /// Console demo
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Console Demo - eStation Developer Edition");
            // Log, static configure
            var logTemplate = "{Timestamp:HH:mm:ss.fff}[{Level:u1}]{Message} {NewLine}{Exception}";

            // Init log
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs/.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: logTemplate,
                    retainedFileCountLimit: 15)
                .WriteTo.Console(
                    outputTemplate: logTemplate)
                .CreateLogger();
            // Init MqttService
            var serverInfor = new ServerInfor();
            Console.Write("Server Port:");
            var inputPort = Console.ReadLine();
            if (string.IsNullOrEmpty(inputPort)) { inputPort = "9071"; }
            if (int.TryParse(inputPort, out var port) && port > 1000 && port < 65535)
            {
                serverInfor.Port = port;
            }

            Console.Write("User Name:");
            var inputUserName = Console.ReadLine();
            if (string.IsNullOrEmpty(inputUserName)) { inputUserName = "test"; }
            serverInfor.UserName = inputUserName;

            Console.Write("Password:");
            var inputPassword = Console.ReadLine();
            if (string.IsNullOrEmpty(inputPassword)) { inputPassword = "123456"; }
            serverInfor.Password = inputPassword;

            Console.Write("Encrypt(Y/N):");
            var inputEncrypt = Console.ReadLine();
            if (string.IsNullOrEmpty(inputEncrypt)) { inputEncrypt = "N"; }
            serverInfor.Encrypt = inputEncrypt.ToUpper() == "Y";
            if (serverInfor.Encrypt)
            {
                Console.Write("Certificate Name:");
                serverInfor.Certificate = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(serverInfor.Certificate))
                {
                    // Check certificate file path logic
                }

                Console.Write("Certificate Key:");
                serverInfor.CertificateKey = Console.ReadLine() ?? string.Empty;
            }

            MQTTService.Instance.Init(serverInfor,
                infor =>        // #Topic: Receive Topic: infor
                {
                    Log.Information($"[Infor]{JsonSerializer.Serialize(infor)}");
                },
                message =>      // #Topic: message
                {
                    Log.Information($"[Message]{JsonSerializer.Serialize(message)}");
                },
                result =>       // #Topic: result
                {
                    Log.Information($"[Result]{JsonSerializer.Serialize(result)}");
                },
                heartbeat =>    // #Topic: heartbeat
                {
                    //Log.Information($"[Heartbeat]{JsonSerializer.Serialize(heartbeat)}");
                }).Wait();

            Console.WriteLine("[0]Push task with base64 string");
            Console.WriteLine("[1]Push task with bytes array");
            Console.WriteLine("[2]Config");

            Console.ReadLine();
            Console.ReadLine(); // Exit
        }

        /// <summary>
        /// #Topic: Push task with base64 stirng
        /// </summary>
        private static void Topic_PushESL()
        {

        }

        /// <summary>
        /// #Topic： Push task with byte array
        /// </summary>
        private static void Topic_Push_ESL2()
        {
            var index = 0;
            var img = SKData.Create("44.bmp");  // Test bitmap: 800*480 (RGBA mode)
            var bmp = SKBitmap.Decode(img);
            var bytes = Compress(bmp.Bytes);
            var prefix = "4400000";     // Test with ETG0750-44
            for (int i = 0; i < 100; i++)
            {
                var list = new List<TagEntity2>();
                for (int j = 0; j < 200; j++)
                {
                    index++;
                    list.Add(new TagEntity2
                    {
                        TagID = prefix + index.ToString("D5"), // From 440000000001 to 440000020000
                        Compress = true,
                        Bytes = bytes,
                        Token = index
                    });
                }
                MQTTService.Instance.PublishMessage("00KQ", "/estation/00KQ/taskESL2", list).Wait();
                Thread.Sleep(5000);
                Log.Information($"Round:{i}-{list.Count}");
            }
        }

        /// <summary>
        /// #Topic: Config
        /// </summary>
        private static void Topic_Config()
        {

        }

        /// <summary>
        /// #Topic: OTA
        /// </summary>
        private static void Topic_OTA()
        {
            // TODO
        }

        /// <summary>
        /// Compress data with GZip
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Compressed data</returns>
        private static byte[] Compress(byte[] source)
        {
            using MemoryStream memory = new();
            using (GZipStream gizp = new(memory, CompressionLevel.Optimal))
            {
                gizp.Write(source, 0, source.Length);
            }
            return memory.ToArray();
        }
    }
}
