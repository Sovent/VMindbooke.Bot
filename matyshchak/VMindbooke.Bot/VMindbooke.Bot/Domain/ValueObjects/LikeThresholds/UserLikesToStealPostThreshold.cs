namespace Usage.Domain.ValueObjects
{
    public class UserLikesToStealPostThreshold
    {
        public UserLikesToStealPostThreshold(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}