using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    public class ChunkCache : IChunkCache
    {
        private readonly IDictionary<Index3, IChunk> _chunks = new Dictionary<Index3, IChunk>();
        private readonly Func<Index3, IChunk> _loadDelegate;
        private readonly Action<Index3, IChunk> _saveDelegate;

        public ChunkCache(Func<Index3, IChunk> loadDelegate, Action<Index3, IChunk> saveDelegate)
        {
            _saveDelegate = saveDelegate;
            _loadDelegate = loadDelegate;
        }

        public IChunk Get(Index3 idx)
        {
            if (!_chunks.ContainsKey(idx))
                return null;

            return _chunks[idx];
        }

        public void EnsureLoaded(Index3 idx)
        {
            if (!_chunks.ContainsKey(idx))
                _chunks[idx] = _loadDelegate(idx);
        }

        public void Release(Index3 idx)
        {
            IChunk chunk;
            if (_chunks.TryGetValue(idx, out chunk))
            {
                _saveDelegate(idx, chunk);
                _chunks.Remove(idx);
            }
        }

        public void Flush()
        {
            foreach (var chunk in _chunks)
              _saveDelegate(chunk.Key, chunk.Value);
        }
    }
}
