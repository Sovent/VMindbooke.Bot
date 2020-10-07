using System.Collections.Generic;

namespace VmindbookeSDK
{
    public class UserRegistrationResponse
    {
        public int id { get; set; }

        public string token{ get; set; }
        public string name{ get; set; }
        public IReadOnlyCollection<object> likes{ get; set; }
    }
}