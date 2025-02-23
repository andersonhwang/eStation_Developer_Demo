using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Demo_Common.Service
{
    /// <summary>
    /// Web service
    /// </summary>
    public class WebService
    {
        private readonly Dictionary<string, string> Firmwares = new();
        private static WebService instance = new();
        /// <summary>
        /// Instance of WebService
        /// </summary>
        public static WebService Instance => instance;

        /// <summary>
        /// Run web service
        /// </summary>
        /// <param name="port">Web port</param>
        public void Run(int port)
        {
            try
            {
                var builder = WebApplication.CreateBuilder();
                builder.Services.AddEndpointsApiExplorer();

                var app = builder.Build();
                app.UseResponseCaching();
                app.MapGet("/test", () => "HelloWorld").WithName("Test");
                app.MapGet("/ota/{key}", (string key) =>
                {
                    try
                    {
                        if (Firmwares.ContainsKey(key))
                        {
                            return Results.File(Path.GetFullPath(Firmwares[key]));
                        }
                        return Results.StatusCode(404);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Down_Error");
                    }
                    return Results.StatusCode(404);
                }).WithName("Ota");
                app.MapGet("/confirm/{key}/{id}", (string key, string id) =>
                {
                    Log.Information($"OTA_{key}_{id}_OK");  // Just mean AP download Firmware OK
                    SendService.Instance.DownloadConfirm(key, id);
                    return Results.Ok(id);
                }).WithName("Confirm");
                app.RunAsync($"http://*:{port}");
            }
            catch (Exception e)
            {
                Log.Error(e, "Web_Svc_Err");
                throw;
            }
        }

        /// <summary>
        /// Add firmware
        /// </summary>
        /// <param name="key">Firmware key</param>
        /// <param name="path">Firmware path</param>
        public void AddItem(string key, string path)
        {
            if (!Firmwares.ContainsKey(key))
            {
                Firmwares.Add(key, path);
            }
            Firmwares[key] = path;
        }
    }
}
