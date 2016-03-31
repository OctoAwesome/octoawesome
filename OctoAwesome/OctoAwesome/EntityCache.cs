using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public sealed class EntityCache
    {
        private Dictionary<Index2, int> passiveCounter = new Dictionary<Index2, int>();

        private Dictionary<Index2, int> referenceCounter = new Dictionary<Index2, int>();

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

        public void Subscribe(Index2 index)
        {
            
            if (!referenceCounter.ContainsKey(index))
            {
                referenceCounter.Add(index, 0);
                // TODO: Entites aus diesem Chunk laden
                // TODO: passiver Subscribe auf die umliegenden Chunks
            }
            referenceCounter[index]++;
        }

        public void Unsubscribe(Index2 index)
        {
            referenceCounter[index]--;
            if (referenceCounter[index] <= 0)
            {
                // TODO: Entities ausladen
                referenceCounter.Remove(index);
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
    }
}
