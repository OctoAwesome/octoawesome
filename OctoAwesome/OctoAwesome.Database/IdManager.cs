using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Database
{
    public sealed class IdManager
    {
        private readonly Queue<int> freeIds;
        private readonly HashSet<int> reservedIds;
        private int nextId;

        public IdManager() : this(Array.Empty<int>())
        {
        }
        public IdManager(IEnumerable<int> alreadyUsedIds)
        {
            if (alreadyUsedIds == null)
                alreadyUsedIds = Array.Empty<int>();

            freeIds = new Queue<int>();
            reservedIds = new HashSet<int>();

            var ids = alreadyUsedIds.Distinct().OrderBy(i => i).ToArray();
            if (ids.Length <= 0)
            {
                nextId = 0;
                return;
            }
            nextId = ids.Max();

            var ids2 = new List<int>(nextId);
            ids2.AddRange(ids);

            for (var i = 0; i < nextId; i++)
            {
                if (i >= ids2.Count || ids2[i] == i)
                    continue;

                ids2.Insert(i, i);
                freeIds.Enqueue(i);
            }
        }

        public int GetId()
        {
            int id;

            do
            {
                id = freeIds.Count > 0 ? freeIds.Dequeue() : nextId++;
            } while (reservedIds.Contains(id));

            return id;
        }

        public void ReleaseId(int id)
        {
            freeIds.Enqueue(id);
            reservedIds.Remove(id);
        }

        public void ReserveId(int id) 
            => reservedIds.Add(id);
    }
}
