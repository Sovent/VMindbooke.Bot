using System.Collections.Generic;

namespace VmindbookeSDK
{
    public interface IVmindbookeClient
    {
        IReadOnlyCollection<User> GetUsers();
    }
}