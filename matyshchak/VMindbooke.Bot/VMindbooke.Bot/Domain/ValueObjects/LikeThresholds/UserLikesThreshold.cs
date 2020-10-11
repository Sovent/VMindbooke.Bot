namespace Usage.Domain.ValueObjects
{
    public class UserLikesThreshold
    {
        public UserLikesThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}