using NUnit.Framework;
using VMindbooke.SDK;

namespace VMindbooke.Bot.Tests
{
    public class VMindbookeClientUnitTests
    {
        [Test]
        public void GetUser_CorrectIdOfFirstUserFromVMindbooke()
        {
            var client = new VMindbookeClient("http://135.181.101.47");
            
            User user = client.GetUser(1);
            
            Assert.AreEqual(1, user.Id);
        }
    }
}