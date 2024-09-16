using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public static class ValidCollectionExtension
    {
        public static IEnumerable<T> GetValidCollection<T>(this IEnumerable<IValidObject> collection)
        {
            var validCollection = new List<T>();

            foreach (var element in collection)
            {
                if (element.IsValid())
                    validCollection.Add((T)element);
            }

            return validCollection;
        }
    }
}