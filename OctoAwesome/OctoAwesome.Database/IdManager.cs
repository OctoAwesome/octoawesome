using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Manager for management of free ids.
    /// </summary>
    public sealed class IdManager
    {
        private readonly Queue<int> freeIds;
        private readonly HashSet<int> reservedIds;
        private int nextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdManager"/> class.
        /// </summary>
        /// <param name="alreadyUsedIds">Pre reserved ids; <c>null</c> to not pre reserve any ids.</param>
        public IdManager(IEnumerable<int>? alreadyUsedIds = null)
        {
            alreadyUsedIds ??= Array.Empty<int>();

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

        /// <summary>
        /// Gets the next free id.
        /// </summary>
        /// <returns>A new id to use.</returns>
        public int GetId()
        {
            int id;

            do
            {
                id = freeIds.Count > 0 ? freeIds.Dequeue() : nextId++;
            } while (reservedIds.Contains(id));

            return id;
        }

        /// <summary>
        /// Release an id to be free to be used again.
        /// </summary>
        /// <param name="id">The id to release.</param>
        public void ReleaseId(int id)
        {
            freeIds.Enqueue(id);
            reservedIds.Remove(id);
        }

        /// <summary>
        /// Reserves a specific id.
        /// </summary>
        /// <param name="id">The id to reserve.</param>
        public void ReserveId(int id)
            => reservedIds.Add(id);
    }
}
