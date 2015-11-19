using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OctoAwesome
{
    // Thread-sicherer Cache mit mehrstufigen Sperren
    // Da Werte verzögert geladen werden, gibt es auch Sperren auf der Ebene einzelner Elemente,
    // anstatt nur für den gesammten Cache.
    // Der Index vom Typ I ist gleichzeitig die Identität des Wertes.
    // Damit wird auch sichergestellt, das Lade- und Speichervorgänge für die gleiche "Entität" sich
    // nicht überlappen können.
    public sealed class Cache<I, V>
    {
        private int size;

        private Dictionary<I, CacheItem> _cache;
        private Dictionary<I, IAsyncResult> _savingStates;

        private Stopwatch watch = new Stopwatch();

        private Func<I, V> loadDelegate;

        private Action<CacheItem> asyncSaveDelegate;

        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim savingStatesLock = new ReaderWriterLockSlim();

        public Cache(int size, Func<I, V> loadDelegate, Action<I, V> saveDelegate)
        {
            if (size < 1)
                throw new ArgumentException();

            if (loadDelegate == null)
                throw new ArgumentNullException();

            this.size = size;
            this.loadDelegate = loadDelegate;
            if (saveDelegate != null)
                this.asyncSaveDelegate = item =>
                {
                    // "Kein Wert" muss wirklich nicht gespeichert werden.
                    // Vermeidet außerdem, dass der verzögerte Ladeprozess sich selbst sperrt,
                    // da er ansonsten auf einen Abschluss dieses asynchron aufgerufenen Delegaten
                    // endlos warten müsste (Deadlock)
                    if (item.ValueLoaded) saveDelegate(item.Index, item.Value);
                };
            _cache = new Dictionary<I, CacheItem>(size + 1);
            _savingStates = new Dictionary<I, IAsyncResult>(size);
            watch.Start();
        }

        public V Get(I index)
        {
            CacheItem item;
            // Rufe CacheItem ohne Sperre ab, da der reine Lesezugriff auf ein Dictionary Thread-sicher ist.
            if (_cache.TryGetValue(index, out item))
            {
                item.LastAccess = watch.Elapsed;
                return item.Value;
            }

            // Holt eine Lesesperre, die in eine Schreibsperre umgewandelt werden kann
            // Achtung: Die Umwandlung in eine Schreibsperre kann nur von dem Thread durchgeführt werden,
            // der sich vorher auch eine umwandelbare Lesesperre geholt hat, ansonsten gibt es Deadlocks!!
            // siehe auch: http://coders-corner.net/2013/05/08/multithreading-in-c-teil-14-readerwriterlockslim/
            using (new UpgradeableGuard(cacheLock))
            {
                // Greife erneut auf das Dictionary mit einer Lesesperre zu, für den Fall
                // das dieser Thread durch die Lesesperre blockiert wurde während das Dictionary
                // von einem anderen Thread gefüllt wurde
                if (_cache.TryGetValue(index, out item))
                    item.LastAccess = watch.Elapsed;
                else // im anderen Fall, dass wieder nichts gefunden wurde, wird ein neues CacheItem erzeugt
                    item = AddCacheItem(index); // die Schreibsperre wird innerhalb von AddCacheItem angefordert
            }

            return item.Value;
        }

        private CacheItem AddCacheItem(I index)
        {
            // Fordert eine Schreibsperre an, die potenziell aus einer Lesesperre umgewandelt wurde.
            using (new WriteGuard(cacheLock))
            {
                CacheItem item = new CacheItem()
                {
                    Index = index,
                    LastAccess = watch.Elapsed,
                    LoadDelegate = LoadValueWaitingForSave
                };

                CacheItem toRemove = null;
                Debug.Assert(!_cache.ContainsKey(index));
                // Es sollte wegen den Sperren nicht notwendig sein vorher zu prüfen,
                // ob das Item schon hinzugefügt wurde.
                //if (!_cache.ContainsKey(index))
                //{
                _cache.Add(index, item);
                //}

                if (_cache.Count > size)
                {
                    toRemove = _cache.Values.OrderBy(v => v.LastAccess).First();
                    using (toRemove.WriteGuard)
                    {
                        _cache.Remove(toRemove.Index);
                        toRemove.ValueNoLongerManagedInCache = true;
                    }
                }

                if (toRemove != null && asyncSaveDelegate != null)
                {
                    I removeIndex = toRemove.Index;
                    CacheItem notsavedItem = null;
                    using (new ReadGuard(savingStatesLock))
                    {
                        IAsyncResult state;
                        if (_savingStates.TryGetValue(removeIndex, out state))
                            notsavedItem = (CacheItem)state.AsyncState;
                    }

                    // Wenn für das zu entfernende Item noch ein alter asynchroner Speichervorgang läuft,
                    // warte bis dieser vollständig abgeschlossen wurde und sich selbst aus
                    // dem Dictionary entfernt hat.
                    if (notsavedItem != null)
                        notsavedItem.ItemSaveCompletedEvent.WaitOne();

                    using (new WriteGuard(savingStatesLock))
                    {
                        // Führe den eigtl. Speichervorgang asynchron aus, weil wir für das Laden
                        // des neuen Items nicht darauf warten müssen. Es sei denn das neue Item
                        // verwendet den gleichen Index wie das alte, dann wird aber an vorheriger Stelle
                        // explizit darauf gewartet, dass der Speichervorgang abgeschlossen wurde.
                        _savingStates.Add(removeIndex, asyncSaveDelegate.BeginInvoke(toRemove, FinishSaving, toRemove));
                    }
                }

                return item;
            }
        }

        private V LoadValueWaitingForSave(I index)
        {
            WaitHandle waitForSave = null;
            using (new ReadGuard(savingStatesLock))
            {
                IAsyncResult state;
                if (_savingStates.TryGetValue(index, out state))
                    waitForSave = state.AsyncWaitHandle; // Dieses WaitHandle ist ausreichend, da
                                                        // nicht auf das Entfernen aus dem Dictionary
                                                        // gewartet werden muss.
            }

            if (waitForSave != null)
                waitForSave.WaitOne();

            return loadDelegate(index);
        }

        private void FinishSaving(IAsyncResult ar)
        {
            CacheItem item = (CacheItem)ar.AsyncState;
            try
            {
                using (new WriteGuard(savingStatesLock))
                {
                    Debug.Assert(_savingStates.Remove(item.Index));
                }

                // Stelle sicher, dass EndInvoke genau einmal für jeden IAsyncResult aufgerufen wird!
                // siehe: http://blog.aggregatedintelligence.com/2010/06/c-asynchronous-programming-using.html
                asyncSaveDelegate.EndInvoke(ar);
            }
            finally
            {
                item.ItemSaveCompletedEvent.Set();
            }
        }

        public void Flush()
        {
            using (var guard = new ReadGuard(cacheLock)) // UpgradeableGuard
            {
                if (asyncSaveDelegate != null)
                {
                    using (new WriteGuard(savingStatesLock))
                    {
                        foreach (var item in _cache.Values)
                        {
                            _savingStates.Add(item.Index, asyncSaveDelegate.BeginInvoke(item, FinishSaving, item));
                        }
                    }
                }

                //guard.UpgradeToWriterLock();
                //_cache.Clear();
            }
        }

        private class CacheItem
        {
            private ReaderWriterLockSlim itemLock = new ReaderWriterLockSlim();
            private AutoResetEvent itemSaveCompletedEvent = new AutoResetEvent(false);
            private V value;
            private bool valueLoaded;

            public EventWaitHandle ItemSaveCompletedEvent { get { return itemSaveCompletedEvent; } }

            public TimeSpan LastAccess { get; set; }

            public Func<I, V> LoadDelegate { get; set; }

            public I Index { get; set; }

            public bool ValueLoaded { get { return valueLoaded; } }
            public bool ValueNoLongerManagedInCache { get; set; }

            public WriteGuard WriteGuard { get { return new WriteGuard(itemLock); } }

            /// <summary>
            /// Der Wert dieses CacheItem wird durch den LoadDelegate und
            /// entsprechenden Lese/Schreibsperren explizit verzögert geladen.
            /// Dies ist die 2. Ebene des Locking-Konzepts
            /// </summary>
            public V Value
            {
                get
                {
                    if (valueLoaded)
                        return value;

                    using (UpgradeableGuard guard = new UpgradeableGuard(itemLock))
                    {
                        if (valueLoaded)
                            return value;

                        guard.UpgradeToWriterLock();

                        value = LoadDelegate(Index);
                        valueLoaded = true;
                        // Schütze vor Zugriffen auf den Wert, wenn nicht sichergestellt werden kann,
                        // dass dieser auch gespeichert wird.
                        if (ValueNoLongerManagedInCache)
                            throw new InvalidOperationException("Der Wert wird nicht länger vom Cache verwaltet und ist dadurch ungültig.");
                        return value;
                    }
                }
            }
        }
    }
}
