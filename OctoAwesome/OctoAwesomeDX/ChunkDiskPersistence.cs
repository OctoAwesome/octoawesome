using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OctoAwesome
{
    public class ChunkDiskPersistence : IChunkPersistence
    {
        public void Save(IChunk chunk, IPlanet planet)
        {
            var root = GetRoot();

            string filename = planet.Id.ToString() + "_" + chunk.Index.X + "_" + chunk.Index.Y + "_" + chunk.Index.Z + ".chunk";
            using (Stream stream = File.Open(root.FullName + "\\" + filename, FileMode.Create, FileAccess.Write))
            {
                chunk.Serialize(stream);
            }
        }

        public IChunk Load(IPlanet planet, Index3 index)
        {
            var root = GetRoot();
            string filename = planet.ToString() + "_" + index.X + "_" + index.Y + "_" + index.Z + ".chunk";

            if (!File.Exists(root.FullName + "\\" + filename))
                return null;

            using (Stream stream = File.Open(root.FullName + "\\" + filename, FileMode.Open, FileAccess.Read))
            {
                IChunk chunk = new Chunk(index, planet);
                chunk.Deserialize(stream, BlockDefinitionManager.GetBlockDefinitions());
                return chunk;
            }
        }

        private DirectoryInfo GetRoot()
        {
            string appconfig = ConfigurationManager.AppSettings["ChunkRoot"];
            if (!string.IsNullOrEmpty(appconfig))
            {
                return new DirectoryInfo(appconfig);
            }
            else
            {
                FileInfo file = new FileInfo(Assembly.GetExecutingAssembly().Location);
                DirectoryInfo root = new DirectoryInfo(file.Directory + "\\OctoMap");
                if (!root.Exists) root.Create();
                return root;
            }
        }
    }
}
