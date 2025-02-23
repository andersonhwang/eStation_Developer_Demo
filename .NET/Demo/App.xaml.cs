using Serilog;
using System.Windows;

namespace Demo_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Init log
            var template = "{Timestamp:HH:mm:ss.fff}[{Level:u1}]{Message} {NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs/.log", rollingInterval: RollingInterval.Day, outputTemplate: template, retainedFileCountLimit: 15)
                .CreateLogger();
        }
    }
}