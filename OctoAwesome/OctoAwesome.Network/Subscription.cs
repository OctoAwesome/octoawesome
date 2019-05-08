using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Subscription<T> : IDisposable
    {
        public IObservable<T> Observable { get; private set; }
        public IObserver<T> Observer { get; private set; }
        
        public Subscription(IObservable<T> observable, IObserver<T> observer)
        {
            Observable = observable;
            Observer = observer;
        }

        public void Dispose()
        {
        }
    }
}
