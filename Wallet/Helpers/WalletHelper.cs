using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Helpers
{
    public class KeyedList<TKey, TItem> : List<TItem>
    {
        public TKey Key { get; protected set; }
        public IEnumerable<TItem> Items { get; protected set; }

        public KeyedList(TKey key, IEnumerable<TItem> items) : base(items)
        {
            Key = key;
            Items = items;
        }
        public KeyedList(IGrouping<TKey, TItem> grouping) : base(grouping)
        {
            Key = grouping.Key;
            Items = grouping;
        }
    }
}
