using Bogus;

namespace Usage.Domain.ContentProviders
{
    public class CommentContentProvider : ICommentContentProvider
    {
        private readonly Faker _faker = new Faker("ru");

        public CommentContent GetCommentContent()
        {
            return new CommentContent(_faker.Lorem.Sentence());
        }
    }
}