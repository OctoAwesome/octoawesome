using System.Collections.Generic;

namespace OctoAwesome
{

    public interface IEntityList : ICollection<Entity>
    {

        IEnumerable<FailEntityChunkArgs> FailChunkEntity();
    }
}
