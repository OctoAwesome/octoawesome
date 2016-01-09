using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ChunkDiskPersistence : IChunkPersistence
    {
        private IChunkSerializer serializer;

        public ChunkDiskPersistence(IChunkSerializer serializer)
        {
            this.serializer = serializer;
        }

        public void Save(int universe, int planet, IChunk chunk)
        {
            var root = GetRoot();

            string filename = planet.ToString() + "_" + chunk.Index.X + "_" + chunk.Index.Y + "_" + chunk.Index.Z +
                              ".chunk";
            using (
                Stream stream = File.Open(root.FullName + Path.DirectorySeparatorChar + filename, FileMode.Create,
                    FileAccess.Write))
            {
                serializer.Serialize(stream, chunk);
            }
        }

        public IChunk Load(int universe, int planet, Index3 index)
        {
            var root = GetRoot();
            string filename = planet.ToString() + "_" + index.X + "_" + index.Y + "_" + index.Z + ".chunk";

            if (!File.Exists(root.FullName + Path.DirectorySeparatorChar + filename))
                return null;

            using (
                Stream stream = File.Open(root.FullName + Path.DirectorySeparatorChar + filename, FileMode.Open,
                    FileAccess.Read))
            {
                return serializer.Deserialize(stream, new PlanetIndex3(planet, index));
            }
        }

        private DirectoryInfo root;

        private DirectoryInfo GetRoot()
        {
            if (root != null)
                return root;

            string appconfig = ConfigurationManager.AppSettings["ChunkRoot"];
            if (!string.IsNullOrEmpty(appconfig))
            {
                root = new DirectoryInfo(appconfig);
                if (!root.Exists) root.Create();
                return root;
            }
            else
            {
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                root = new DirectoryInfo(exePath + Path.DirectorySeparatorChar + "OctoMap");
                if (!root.Exists) root.Create();
                return root;
            }
        }
    }
}