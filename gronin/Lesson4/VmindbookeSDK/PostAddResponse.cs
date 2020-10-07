using System.Collections.Generic;

namespace VmindbookeSDK
{
    public class PostAddResponse
    {
        public int id { get;set; }

        public string token{ get;set; }
        public string name{ get;set; }
        public IEnumerable<object> likes{ get;set; }
    }
}