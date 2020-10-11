using Serilog;

namespace LikesCheating.Infrastructure
{
    public class Logger : ILogger
    {
        public Logger(string logFileName)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFileName)
                .WriteTo.Console()
                .CreateLogger();
        }
        public void Debug(string text) { Log.Debug(text); }
        public void Information(string text) { Log.Information(text); }
        public void Warning(string text) { Log.Warning(text); }
        public void Error(string text) { Log.Error(text); }
        public void Fatal(string text) { Log.Fatal(text); }
    }
}