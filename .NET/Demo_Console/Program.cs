using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Service;
using Serilog;
using SkiaSharp;
using System.IO.Compression;
using System.Text.Json;

namespace Demo_Console
{
    /// <summary>
    /// Console demo
    /// </summary>
    internal class Program
    {
        static int Token = 0;
        static string LogTemplate = "{Timestamp:HH:mm:ss.fff}[{Level:u1}]{Message} {NewLine}{Exception}";
        static string APID = "009Z";
        static string TopicConfig = "/estation/009Z/config";        // 0x01
        static string TopicPushESL = "/estation/009Z/taskESL";      // 0x02
        static string TopicPushESL2 = "/estation/009Z/taskESL2";    // 0x03
        static string TopicPushDSL = "/estation/009Z/taskDSL";      // 0x04
        static string TopicFirmware = "/estation/009Z/firmware";    // 0x05
        static string TopicOTA = "/estation/009Z/ota";              // 0x06
        static string TestBmp0 = "T0.bmp";  // ET0290, B/W/R, 3D, 2.9inch, 296x128
        static string TestBmp1 = "T1.bmp";  // ET0420, B/W/R, 4.2inch, 400x300
        static string TestBin2 = "T2.bin";
        static string TestBin3 = "T3.bin";
        static string TestBin4 = "T4.bin";

        static ConnInfo Conn = new()
        {
            UserName = "test",
            Password = "123456",
            Port = 9081,
            Encrypt = false,
        };

        static string[] ESL_LST = [
            "3D000075035E",
            "3D00000101DF",
            "3D00000102D3",
            ];

        static string[] DSL_LST = [
            "D000003416D2",
            "D0000034167C",
            "D00000341783",
            "D000003417D4",
            "D000003418D1",
            "D0000034156B",
            "D000003418AC",
            "D000003410E8",
            "D000003416C8",
            ];
        static void Main(string[] args)
        {
            Console.WriteLine("Console Demo - eStation Developer Edition");
            // Init log
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs/.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: LogTemplate,
                    retainedFileCountLimit: 15)
                .WriteTo.Console(
                    outputTemplate: LogTemplate)
                .CreateLogger();
            // Init MqttService

            SendService.Instance.Run(Conn);
            SendService.Instance.Register(ApStatusHandler);
            SendService.Instance.Register(ApHeartbeatHandler);
            SendService.Instance.Register(ApInforHandler);
            SendService.Instance.Register(ApMessageHandler);
            SendService.Instance.Register(TaskResultHandler);

            Console.WriteLine("[1]Config");
            Console.WriteLine("[2]Push task with base64 string");
            Console.WriteLine("[3]Push task with bytes array");
            Console.WriteLine("[4]Push task with bin file");
            Console.WriteLine("[5]OTA AP/MOD or push firmware");
            Console.WriteLine("[6]OTA ESL/DSL");
            Console.WriteLine("[E]Exit");

            var random = new Random(DateTime.Now.Microsecond);

        TEST:
            var code = Console.ReadLine() ?? string.Empty;
            var result = SendResult.Unknown;
            Token = random.Next(0xFFFF);
            switch (code.ToUpper())
            {
                case "1":
                    result = Config();
                    break;
                case "2":
                    result = PushESL();
                    break;
                case "3":
                    result = PushESL2();
                    break;
                case "4":
                    result = PushDSL();
                    break;
                case "5":
                    result = OTA();
                    break;
                case "E":
                    return;
                default:
                    Console.WriteLine("Invalid input:" + code);
                    break;
            }

            Console.WriteLine("Send result:" + result.ToString());
            goto TEST;
        }

        /// <summary>
        /// #Topic: Config
        /// </summary>
        private static SendResult Config()
        {
            try
            {
                var img = SKData.Create("T0.bmp");
                var bmp = SKBitmap.Decode(img);
                var bytes = Compress(bmp.Bytes);
                var list = new List<ESLEntity2>();
                foreach (var id in ESL_LST)
                {
                    list.Add(new ESLEntity2
                    {
                        TagID = id,
                        Compress = true,
                        Bytes = bytes,
                        Token = Token
                    });
                }
                return SendService.Instance.Send(APID, 0x01, TopicConfig, list).Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Config_Error");
                return SendResult.Error;
            }
        }

        /// <summary>
        /// #Topic: Push task with base64 stirng
        /// </summary>
        private static SendResult PushESL()
        {
            try
            {
                var img = SKData.Create(TestBmp0);
                var bmp = SKBitmap.Decode(img);
                var base64 = Convert.ToBase64String(bmp.Bytes);
                var list = new List<ESLEntity>();
                foreach (var id in ESL_LST)
                {
                    list.Add(new ESLEntity
                    {
                        TagID = id,
                        Base64String = base64,
                        Token = Token
                    });
                }
                return SendService.Instance.Send(APID, 0x02, TopicPushESL, list).Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Push_ESL_Error");
                return SendResult.Error;
            }
        }

        /// <summary>
        /// #Topic： Push task with byte array
        /// </summary>
        private static SendResult PushESL2()
        {
            try
            {
                var img = SKData.Create(TestBmp0);
                var bmp = SKBitmap.Decode(img);
                var bytes = Compress(bmp.Bytes);
                var list = new List<ESLEntity2>();
                foreach (var id in ESL_LST)
                {
                    list.Add(new ESLEntity2
                    {
                        TagID = id,
                        Compress = true,
                        Bytes = bytes,
                        Token = Token
                    });
                }
                return SendService.Instance.Send(APID, 0x03, TopicPushESL2, list).Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Push_ESL2_Error");
                return SendResult.Error;
            }
        }

        /// <summary>
        /// #Topic: Push task with bin data
        /// </summary>
        private static SendResult PushDSL()
        {
            try
            {
                var bytes = File.ReadAllBytes(TestBin2);
                var list = new List<DSLEntity>();
                foreach (var id in DSL_LST)
                {
                    list.Add(new DSLEntity
                    {
                        TagID = id,
                        HexData = bytes,
                        Token = Token
                    });
                }
                return SendService.Instance.Send(APID, 0x04, TopicPushDSL, list).Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Push_DSL_Error");
                return SendResult.Error;
            }
        }

        /// <summary>
        /// #Topic: OTA
        /// </summary>
        private static SendResult OTA()
        {
            // TODO
            return SendResult.Unknown;
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

        /// <summary>
        /// AP status handler
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="status">AP status</param>
        private static void ApStatusHandler(string id, ApStatus status)
        {
            Log.Information($"AP:{id}, Status:{status}.");
        }

        /// <summary>
        /// AP heartbeat handler
        /// </summary>
        /// <param name="heartbeat">AP heartbeat</param>
        private static void ApHeartbeatHandler(ApHeartbeat heartbeat)
        {
            // Log.Information($"[AP Heartbeat]{JsonSerializer.Serialize(heartbeat)}");
        }

        /// <summary>
        /// AP infor handler
        /// </summary>
        /// <param name="infor">AP infor</param>
        private static void ApInforHandler(ApInfor infor)
        {
            Log.Information($"[AP Infor]{JsonSerializer.Serialize(infor)}");
        }

        /// <summary>
        /// AP message handler
        /// </summary>
        /// <param name="message">AP message</param>
        private static void ApMessageHandler(ApMessage message)
        {
            Log.Information($"[AP Message]{JsonSerializer.Serialize(message)}");
        }

        /// <summary>
        /// Task result handler
        /// </summary>
        /// <param name="result">Task result</param>
        private static void TaskResultHandler(TaskResult result)
        {
            Log.Information($"[Task Result]{JsonSerializer.Serialize(result)}");
        }
    }
}
