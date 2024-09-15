using System.IO;
using Newtonsoft.Json;

namespace LikesCheating.Infrastructure
{
    public class ActionsRepository
    {
        public static void Save<T>(string path, T objectToSave)
        {
            using (var stream = new StreamWriter(path, append: false))
            {
                stream.Write(JsonConvert.SerializeObject(objectToSave));   
            }
        }

        public static T Load<T>(string path)
        {
            using (var stream = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
            }
        }
    }
}