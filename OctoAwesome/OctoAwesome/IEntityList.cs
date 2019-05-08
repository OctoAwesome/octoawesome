using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public interface IEntityList : ICollection<Entity>
    {
        IEnumerable<FailEntityChunkArgs> FailChunkEntity();
    }

    public class FailEntityChunkArgs
    {
        public Index2 CurrentChunk { get; set; }
        public int CurrentPlanet { get; set; }

        public Index2 TargetChunk { get; set; }
        public int TargetPlanet { get; set; }

        public Entity Entity { get; set; }
    }
}
