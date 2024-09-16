namespace VMindbooke.Bot.Domain
{
    public interface IHashesRepository
    {
        bool DoesContainPostHashWith(int postId);
        
        bool DoesContainUserHashWith(int userId);

        int GetPostHashBy(int postId);

        int GetUserHashBy(int userId);

        void AddOrUpdatePostHash(int postId, int postHash);

        void AddOrUpdateUserHash(int userId, int userHash);
    }
}