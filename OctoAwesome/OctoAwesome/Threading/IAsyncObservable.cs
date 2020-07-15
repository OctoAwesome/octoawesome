using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    public interface IAsyncObservable<T>
    {
        /// <summary>
        ///  Benachrichtigt den Anbieter, dass ein Beobachter Benachrichtigungen empfangen soll.
        /// </summary>
        /// <param name="observer">Das Objekt, das Benachrichtigungen empfangen soll.</param>
        /// <returns>Die Schnittstelle des Beobachters, die das Freigeben von Ressourcen ermöglicht.</returns>
        Task<IDisposable> Subscribe(IAsyncObserver<T> observer);
    }
}
