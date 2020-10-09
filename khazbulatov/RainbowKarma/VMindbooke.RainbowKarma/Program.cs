using Serilog;

namespace VMindbooke.RainbowKarma
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("rainbow.log")
                .WriteTo.Console()
                .CreateLogger();
            
            new RainbowKarmaBot().Run();
        }
    }
}
