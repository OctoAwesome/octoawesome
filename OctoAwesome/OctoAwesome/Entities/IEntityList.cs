using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Interface for <see cref="IEntityList"/>
    /// </summary>
    public interface IEntityList : ICollection<Entity>
    {        
        /// <summary>
        /// Checks if an <see cref="Entity"/> leaves a <see cref="IChunkColumn"/>?
        /// </summary>
        /// <returns></returns>
        IEnumerable<FailEntityChunkArgs> FailChunkEntity();
    }
}
