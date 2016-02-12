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
    public class DiskPersistenceManager : IPersistenceManager
    {
        private IUniverseSerializer universeSerializer;
        private IPlanetSerializer planetSerializer;
        private IColumnSerializer columnSerializer;

        public DiskPersistenceManager(IUniverseSerializer universeSerializer, 
            IPlanetSerializer planetSerializer, 
            IColumnSerializer columnSerializer)
        {
            this.universeSerializer = universeSerializer;
            this.planetSerializer = planetSerializer;
            this.columnSerializer = columnSerializer;
        }

        private DirectoryInfo root;

        private string GetRoot()
        {
            if (root != null)
                return root.FullName;

            string appconfig = ConfigurationManager.AppSettings["ChunkRoot"];
            if (!string.IsNullOrEmpty(appconfig))
            {
                root = new DirectoryInfo(appconfig);
                if (!root.Exists) root.Create();
                return root.FullName;
            }
            else
            {
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                root = new DirectoryInfo(exePath + Path.DirectorySeparatorChar + "OctoMap");
                if (!root.Exists) root.Create();
                return root.FullName;
            }
        }

        public void SaveUniverse(IUniverse universe)
        {
            string file = Path.Combine(GetRoot(), universe.Id.ToString(), "universe.info");

            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                universeSerializer.Serialize(stream, universe);
            }
        }

        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString(), "planet.dat");

            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                planetSerializer.Serialize(stream, planet);
            }
        }

        public void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString());
            Directory.CreateDirectory(path);

            string file = path = Path.Combine(path, string.Format("column_{0}_{1}.dat", column.Index.X, column.Index.Y));
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                columnSerializer.Serialize(stream, column);
            }
        }

        public IUniverse[] ListUniverses()
        {
            //string path = Path.Combine(GetRoot(), universe.ToString(), planet.ToString());

            //using (Stream stream = File.Open(root.FullName + Path.DirectorySeparatorChar + filename, FileMode.Open, FileAccess.Read))
            //{
            //    return serializer.Deserialize(stream, new PlanetIndex3(planet, index));
            //}

            throw new NotImplementedException();
        }

        public IUniverse LoadUniverse(Guid universeGuid)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), "universe.dat");
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                return universeSerializer.Deserialize(stream);
            }
        }

        public IPlanet LoadPlanet(Guid universeGuid, int planetId)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), "planet.dat");
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                return planetSerializer.Deserialize(stream);
            }

        }

        public IChunkColumn LoadColumn(Guid universeGuid, int planetId, Index2 columnIndex)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), string.Format("column_{0}_{1}.dat", columnIndex.X, columnIndex.Y));
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                return columnSerializer.Deserialize(stream);
            }

        }
    }
}
