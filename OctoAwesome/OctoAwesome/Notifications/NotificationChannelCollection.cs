using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public class NotificationChannelCollection : IEnumerable<KeyValuePair<string, HashSet<INotificationObserver>>>
    {
        public HashSet<INotificationObserver> this[string channel] => internalDictionary[channel];

        public ICollection<string> Channels => internalDictionary.Keys;

        public int Count => internalDictionary.Count;

        public Dictionary<string, HashSet<INotificationObserver>>.ValueCollection Values => internalDictionary.Values;

        private readonly Dictionary<string, HashSet<INotificationObserver>> internalDictionary;

        public NotificationChannelCollection()
        {
            internalDictionary = new Dictionary<string, HashSet<INotificationObserver>>();
        }

        public void Add(string channel, INotificationObserver value)
        {
            if (internalDictionary.TryGetValue(channel, out var hashset))
                hashset.Add(value);
            else
                internalDictionary.Add(channel, new HashSet<INotificationObserver> { value });
        }

        public void Clear()
            => internalDictionary.Clear();

        public bool Contains(INotificationObserver item)
            => internalDictionary.Values.Any(i => i == item);
        public bool Contains(string key)
            => internalDictionary.ContainsKey(key);

        public Dictionary<string, HashSet<INotificationObserver>>.Enumerator GetEnumerator()
            => internalDictionary.GetEnumerator();

        public bool Remove(string key)
            => internalDictionary.Remove(key);
        public bool Remove(INotificationObserver item)
        {
            var returnValue = false;

            foreach (var hashSet in internalDictionary.Values)
                returnValue = returnValue ? returnValue : hashSet.Remove(item);

            return returnValue;
        }
        public bool Remove(string key, INotificationObserver item)
            => internalDictionary[key].Remove(item);

        public bool TryGetValue(string key, out HashSet<INotificationObserver> value)
            => internalDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => internalDictionary.GetEnumerator();
        IEnumerator<KeyValuePair<string, HashSet<INotificationObserver>>> IEnumerable<KeyValuePair<string, HashSet<INotificationObserver>>>.GetEnumerator()
            => internalDictionary.GetEnumerator();
    }
}
