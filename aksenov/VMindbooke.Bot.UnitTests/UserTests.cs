using NUnit.Framework;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.UnitTests
{
    public class UserTests
    {
        [Test]
        public void IsValid_NotValidUserObjectInInput()
        {
            //arrange
            var user = new User(1, "token", null, new Like[0]);
            
            //act
            var isValid = user.IsValid();

            //assert
            Assert.IsFalse(isValid);
        }
    }
}