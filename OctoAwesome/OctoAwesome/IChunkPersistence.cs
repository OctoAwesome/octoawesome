using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für das Persistieren eines Chunks
    /// </summary>
    public interface IChunkPersistence
    {
        /// <summary>
        /// Methode die den Chunk speichert
        /// </summary>
        /// <param name="universe">Das Universum des Chunks</param>
        /// <param name="planet">Der Planent des Chunks</param>
        /// <param name="chunk">Der Chunk, der gespeichert werden soll</param>
        void Save(int universe, int planet, IChunk chunk);

        /// <summary>
        /// Methode, die den Chunk lädt
        /// </summary>
        /// <param name="universe">Das Universum in dem sich der Chunk befindet</param>
        /// <param name="planet">Der Planet des Chunks</param>
        /// <param name="index">Die Koordinaten des Chunks</param>
        /// <returns>Der geladene Chunk</returns>
        IChunk Load(int universe, int planet, Index3 index);
    }
}