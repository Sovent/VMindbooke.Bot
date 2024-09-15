namespace LikesCheating.Infrastructure
{
    public interface IConfiguration
    {
        string GetValue(string key);
    }
}