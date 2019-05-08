using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public class NotificationChannelCollection : IEnumerable<KeyValuePair<string, ObserverHashSet>>
    {
        public ObserverHashSet this[string channel] => internalDictionary[channel];

        public ICollection<string> Channels => internalDictionary.Keys;

        public int Count => internalDictionary.Count;

        public Dictionary<string, ObserverHashSet>.ValueCollection Values => internalDictionary.Values;

        private readonly Dictionary<string, ObserverHashSet> internalDictionary;

        public NotificationChannelCollection() 
            => internalDictionary = new Dictionary<string, ObserverHashSet>();

        public void Add(string channel, INotificationObserver value)
        {
            if (internalDictionary.TryGetValue(channel, out ObserverHashSet hashset))
            {
                hashset.Wait();
                hashset.Add(value);
                hashset.Release();
            }
            else
            {
                internalDictionary.Add(channel, new ObserverHashSet { value });
            }
        }

        public void Clear()
            => internalDictionary.Clear();

        public bool Contains(INotificationObserver item)
            => internalDictionary.Values.Any(i => i == item);
        public bool Contains(string key)
            => internalDictionary.ContainsKey(key);

        public Dictionary<string, ObserverHashSet>.Enumerator GetEnumerator()
            => internalDictionary.GetEnumerator();

        public bool Remove(string key)
            => internalDictionary.Remove(key);
        public bool Remove(INotificationObserver item)
        {
            var returnValue = false;

            foreach (ObserverHashSet hashSet in internalDictionary.Values)
            {
                hashSet.Wait();
                returnValue = returnValue ? returnValue : hashSet.Remove(item);
                hashSet.Release();
            }

            return returnValue;
        }
        public bool Remove(string key, INotificationObserver item)
        {
            ObserverHashSet hashSet = internalDictionary[key];
            hashSet.Wait();
            var returnValue = hashSet.Remove(item);
            hashSet.Release();
            return returnValue;
        }

        public bool TryGetValue(string key, out ObserverHashSet value)
            => internalDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => internalDictionary.GetEnumerator();
        IEnumerator<KeyValuePair<string, ObserverHashSet>> IEnumerable<KeyValuePair<string, ObserverHashSet>>.GetEnumerator()
            => internalDictionary.GetEnumerator();
    }
}
