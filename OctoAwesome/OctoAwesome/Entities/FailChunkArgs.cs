using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Entities
{

    /// <summary>
    /// <see cref="FailEntityChunkArgs"/>.
    /// </summary>
    public class FailEntityChunkArgs
    {
        /// <summary>
        /// Position of Chuck.
        /// </summary>
        public Index2 CurrentChunk { get; set; }
        /// <summary>
        /// Chunk Plante.
        /// </summary>
        public int CurrentPlanet { get; set; }
        /// <summary>
        /// Target Chunk
        /// </summary>
        public Index2 TargetChunk { get; set; }
        /// <summary>
        /// Target Planet
        /// </summary>
        public int TargetPlanet { get; set; }
        /// <summary>
        /// Entity
        /// </summary>
        public Entity Entity { get; set; }
    }
}
