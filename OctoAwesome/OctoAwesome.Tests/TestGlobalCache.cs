using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctoAwesome.Notifications;

namespace OctoAwesome.Tests
{
    internal class TestGlobalCache : IGlobalChunkCache
    {
        public int LoadCounter { get; private set; }

        public int SaveCounter { get; private set; }

        public List<PlanetIndex3> Loaded { get; private set; }

        public int LoadedChunkColumns
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int DirtyChunkColumn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IPlanet Planet => throw new NotImplementedException();

        public TestGlobalCache()
        {
            Loaded = new List<PlanetIndex3>();
        }

        public event EventHandler<IChunkColumn> ChunkColumnChanged;

        public void Reset()
        {
            LoadCounter = 0;
            SaveCounter = 0;
            Loaded.Clear();
        }

        public void Release(int planet,Index2 position, bool passiv)
        {
            SaveCounter++;
        }

        public IChunkColumn Subscribe(IPlanet planet, Index2 position,bool passiv)
        {
            LoadCounter++;
            return new ChunkColumn(new IChunk[] {new Chunk(new Index3(position,0),planet),new Chunk(new Index3(position,1),planet),new Chunk(new Index3(position,2),planet) },planet, position);
        }

        public IChunkColumn Peek(int planet, Index2 position)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IPlanet GetPlanet(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsChunkLoaded(int planet, Index2 position)
        {
            throw new NotImplementedException();
        }

        public void BeforeSimulationUpdate(Simulation simulation)
        {
            throw new NotImplementedException();
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(SerializableNotification notification) => throw new NotImplementedException();
        public void Update(SerializableNotification notification) => throw new NotImplementedException();
        public void InsertUpdateHub(IUpdateHub updateHub) => throw new NotImplementedException();
        public void OnCompleted() => throw new NotImplementedException();
        public void OnError(Exception error) => throw new NotImplementedException();
        public void OnNext(Notification value) => throw new NotImplementedException();
        public IChunkColumn Subscribe(Index2 position, bool passive) => throw new NotImplementedException();
        public bool IsChunkLoaded(Index2 position) => throw new NotImplementedException();
        public IChunkColumn Peek(Index2 position) => throw new NotImplementedException();
        public void Release(Index2 position, bool passive) => throw new NotImplementedException();
    }
}
