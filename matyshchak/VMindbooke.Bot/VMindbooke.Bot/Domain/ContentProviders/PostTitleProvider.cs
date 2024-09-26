using Bogus;

namespace Usage.Domain.ContentProviders
{
    public class PostTitleProvider : IPostTitleProvider
    {
        private readonly Faker _faker = new Faker("ru");
        
        public string GetPostTitle()
        {
            return _faker.Commerce.ProductName();
        }
    }
}