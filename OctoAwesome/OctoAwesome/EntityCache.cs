using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public sealed class EntityCache
    {
        private object lockObject = new object();

        private Dictionary<PlanetIndex2, int> passiveCounter = new Dictionary<PlanetIndex2, int>();

        private Dictionary<PlanetIndex2, int> referenceCounter = new Dictionary<PlanetIndex2, int>();

        private Dictionary<PlanetIndex2, LoaderState> loaderStates = new Dictionary<PlanetIndex2, LoaderState>();

        private List<X> Entries { get; set; }

        public EntityCache()
        {
            Entries = new List<X>();
        }

        public void Update()
        {
            foreach (var Entity in Entries)
            {
                // TODO: Update
                Entity.Update();

                // Evtl. nur Entites simulieren die nicht im Grenzbereich stehen?
            }
        }

        public void Subscribe(IPlanet planet, Index2 index, int activationRange)
        {
            int softBorder = 1;

            lock (lockObject)
            {
                for (int x = -activationRange - softBorder; x <= activationRange + softBorder; x++)
                {
                    for (int y = -activationRange - softBorder; y <= activationRange + softBorder; y++)
                    {
                        Index2 pos = new Index2(index.X + x, index.Y + y);
                        pos.NormalizeXY(planet.Size);

                        PlanetIndex2 i = new PlanetIndex2(planet.Id, pos);

                        // Hard Reference
                        if (x >= -activationRange ||
                            x <= activationRange ||
                            y >= -activationRange ||
                            y <= activationRange)
                        {
                            if (!referenceCounter.ContainsKey(i))
                                referenceCounter.Add(i, 0);
                            referenceCounter[i]++;
                        }

                        // Soft Reference
                        if (!passiveCounter.ContainsKey(i))
                        {
                            passiveCounter.Add(i, 0);

                            // Entites aus diesem Chunk laden
                            if (!loaderStates.ContainsKey(i))
                                loaderStates.Add(i, LoaderState.ToLoad);
                            else
                            {
                                switch (loaderStates[i])
                                {
                                    case LoaderState.ToUnload:
                                        loaderStates[i] = LoaderState.Ready;
                                        break;
                                    case LoaderState.Unloading:
                                        loaderStates[i] = LoaderState.CancelUnload;
                                        break;
                                }
                            }
                        }
                        passiveCounter[i]++;
                    }
                }
            }
        }

        public void Unsubscribe(IPlanet planet, Index2 index, int activationRange)
        {
            int softBorder = 1;

            lock (lockObject)
            {

                for (int x = -activationRange - softBorder; x <= activationRange + softBorder; x++)
                {
                    for (int y = -activationRange - softBorder; y <= activationRange + softBorder; y++)
                    {
                        Index2 pos = new Index2(index.X + x, index.Y + y);
                        pos.NormalizeXY(planet.Size);

                        PlanetIndex2 i = new PlanetIndex2(planet.Id, pos);

                        // Hard Reference
                        if (x >= -activationRange ||
                            x <= activationRange ||
                            y >= -activationRange ||
                            y <= activationRange)
                        {
                            referenceCounter[i]--;
                            if (referenceCounter[i] <= 0)
                                referenceCounter.Remove(i);
                        }

                        // Soft Reference
                        passiveCounter[i]--;
                        if (passiveCounter[i] <= 0)
                        {
                            // Entities ausladen
                            switch (loaderStates[i])
                            {
                                case LoaderState.CancelUnload:
                                    loaderStates[i] = LoaderState.Unloading;
                                    break;
                                case LoaderState.ToUnload:
                                    loaderStates[i] = LoaderState.Ready;
                                    break;
                            }
                            
                            passiveCounter.Remove(i);
                        }
                    }
                }
            }
        }

        private void LoaderLoop()
        {
            while (true)
            {
                PlanetIndex2? toload = null;
                lock (lockObject)
                {
                    // Das Element das als nächstes geladen werden soll ermitteln
                    if (loaderStates.Where(s => s.Value == LoaderState.ToLoad).Any())
                    {
                        toload = loaderStates.Where(s => s.Value == LoaderState.ToLoad).First().Key;
                        loaderStates[toload.Value] = LoaderState.Loading;
                    }
                }

                if (toload.HasValue)
                {
                    // Wenn es was zu laden gibt...
                    // TODO: Load toload

                    lock (lockObject)
                    {
                        loaderStates[toload.Value] = LoaderState.Ready;
                    }
                }
                else
                {
                    // Wenn es nichts zu laden gibt...
                    PlanetIndex2? tounload = null;
                    lock (lockObject)
                    {
                        if (loaderStates.Where(s => s.Value == LoaderState.ToUnload).Any())
                        {
                            toload = loaderStates.Where(s => s.Value == LoaderState.ToUnload).First().Key;
                            loaderStates[toload.Value] = LoaderState.Unloading;
                        }
                    }

                    if (tounload.HasValue)
                    {
                        // Wenn es was zum entladen gibt
                        // TODO: Save

                        lock (lockObject)
                        {
                            if (loaderStates[tounload.Value] == LoaderState.CancelUnload)
                            {
                                loaderStates[tounload.Value] = LoaderState.Ready;
                            }
                            else
                            {
                                loaderStates.Remove(tounload.Value);
                                // TODO: Delete Entites
                            }
                        }
                    }
                }
            }
        }

        private class X
        {
            public Index2 Position { get; set; }

            public Entity Entity { get; set; }

            public LocalChunkCache Cache { get; set; }

            public void Update()
            {
                // TODO: Entity Movement
                // TODO: evtl. ChunkMove
                //  -> Freeze, falls neuer Chunk nicht in der Liste
                //  -> Cache aktualisieren

                /*
                    Frage: was passiert, wenn Entity sich aus dem geladenen bereich bewegt?
                    a) Freeze im alten Chunk (leider wirds dann weiterhin simuliert)
                    b) Zielchunk (Entities laden), Entity bewegen, Zielchunk wieder entladen
                    c) Grenzchunks zwar laden, aber nicht simulieren
                */
            }
        }

        private enum LoaderState
        {
            ToLoad,
            Loading,
            Ready,
            ToUnload,
            Unloading,
            CancelUnload,
        }
    }
}
