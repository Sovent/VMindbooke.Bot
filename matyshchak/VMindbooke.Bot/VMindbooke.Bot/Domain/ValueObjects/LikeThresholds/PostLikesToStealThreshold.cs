namespace Usage.Domain.ValueObjects
{
    public class PostLikesToStealThreshold
    {
        public PostLikesToStealThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}