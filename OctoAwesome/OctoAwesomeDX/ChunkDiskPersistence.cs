using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class ChunkDiskPersistence : IChunkPersistence
    {
        private readonly string Root = @"D:\OctoMap\";

        public void Save(IChunk chunk, int planet)
        {
            string filename = planet.ToString() + "_" + chunk.Index.X + "_" + chunk.Index.Y + "_" + chunk.Index.Z + ".chunk";
            using (Stream stream = File.Open(Root + filename, FileMode.Create, FileAccess.Write))
            {
                chunk.Serialize(stream);
            }
        }

        public IChunk Load(int planet, Index3 index)
        {
            string filename = planet.ToString() + "_" + index.X + "_" + index.Y + "_" + index.Z + ".chunk";

            if (!File.Exists(Root + filename))
                return null;

            using (Stream stream = File.Open(Root + filename, FileMode.Open, FileAccess.Read))
            {
                IChunk chunk = new Chunk(index);
                chunk.Deserialize(stream, BlockDefinitionManager.GetBlockDefinitions());
                return chunk;
            }


        }
    }
}
