namespace LikesCheating.Infrastructure
{
    public interface ILogger
    {
        void Debug(string text);
        void Information(string text);
        void Warning(string text);
        void Error(string text);
        void Fatal(string text);
    }
}