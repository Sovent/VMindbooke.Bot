namespace VMindbooster
{
    public class ThresholdConfiguration
    {
        public ThresholdConfiguration(int likedPostThreshold, int likedCommentThreshold)
        {
            LikedPostThreshold = likedPostThreshold;
            LikedCommentThreshold = likedCommentThreshold;
        }

        public int LikedPostThreshold { get; }
        
        public int LikedCommentThreshold { get; }
    }
}