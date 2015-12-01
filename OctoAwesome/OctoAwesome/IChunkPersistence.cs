using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für einen 
    /// </summary>
    public interface IChunkPersistence
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="universe"></param>
        /// <param name="planet"></param>
        /// <param name="chunk"></param>
        void Save(int universe, int planet, IChunk chunk);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="universe"></param>
        /// <param name="planet"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        IChunk Load(int universe, int planet, Index3 index);
    }
}
