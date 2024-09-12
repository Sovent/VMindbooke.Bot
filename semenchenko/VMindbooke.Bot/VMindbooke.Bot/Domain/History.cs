using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class History<TIdType>
    {
        public History()
        {
            Ids = new HashSet<TIdType>();
        }

        private HashSet<TIdType> Ids { get; }

        public bool IsStored(TIdType id)
        {
            return Ids.Contains(id);
        }

        public void MarkAsStored(TIdType id)
        {
            Ids.Add(id);
        }
    }
}