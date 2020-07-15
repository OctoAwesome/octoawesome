using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    public interface IAsyncObserver<T>
    {
        /// <summary>
        ///     Stellt neue Daten für den Beobachter bereit.
        /// </summary>
        /// <param name="value">Die aktuellen Benachrichtigungsinformationen.</param>
        Task OnNext(T value);

        /// <summary>
        ///  Benachrichtigt den Beobachter, dass beim Anbieter ein Fehlerzustand aufgetreten ist.
        /// </summary>
        /// <param name="error">Ein Objekt, das zusätzliche Informationen zum Fehler bereitstellt.</param>
        Task OnError(Exception error);

        /// <summary>
        /// Benachrichtigt den Beobachter, dass der Anbieter aufgehört hat, Pushbenachrichtigungen zu senden.
        /// </summary>
        Task OnCompleted();
    }
}
