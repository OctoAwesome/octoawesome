using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class ChunkLoader : IChunkLoader
    {
        private readonly IChunkCache _cache;
        private readonly int _maxRange;
        private PlanetIndex3 _center;
        private Task _loadingTask;
        private CancellationTokenSource _cancellationToken;

        public ChunkLoader(IChunkCache cache, int range, PlanetIndex3 center)
        {
            _center = center;
            _maxRange = range;
            _cache = cache;
        }

        public void UpdatePosition(int planet, int i, int j, int k)
        {
            _center = new PlanetIndex3(planet, _center.ChunkIndex + new Index3(i, j, k));

            if (_loadingTask != null && !_loadingTask.IsCompleted)
            {
                _cancellationToken.Cancel();
                _cancellationToken = new CancellationTokenSource();
                _loadingTask = _loadingTask.ContinueWith(_ => Reload(_cancellationToken.Token));
            }
            else
            {
                _cancellationToken = new CancellationTokenSource();
                _loadingTask = Task.Factory.StartNew(() => Reload(_cancellationToken.Token));
            }
        }

        private void Reload(CancellationToken token)
        {
            _cache.EnsureLoaded(_center);

            for (int i = 0; i < ChunkCache.RangeX; i++)
                for (int j = 0; j < ChunkCache.RangeY; j++)
                    for (int k = 0; k < ChunkCache.RangeZ; k++)
                    {
                        if (i < _maxRange && j < _maxRange && k < _maxRange)
                            continue;

                        _cache.Release(_center.ChunkIndex.X + i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z + k);
                        _cache.Release(_center.ChunkIndex.X + i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z + k);
                        _cache.Release(_center.ChunkIndex.X - i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z + k);
                        _cache.Release(_center.ChunkIndex.X - i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z + k);
                        _cache.Release(_center.ChunkIndex.X + i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z - k);
                        _cache.Release(_center.ChunkIndex.X + i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z - k);
                        _cache.Release(_center.ChunkIndex.X - i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z - k);
                        _cache.Release(_center.ChunkIndex.X - i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z - k);
                    }

            for (int range = 1; range < _maxRange; range ++) 
                 for (int i = 0; i <= range; i++) 
                     for (int j = 0; j <= range; j++)
                         for (int k = 0; k < range; k++)
                         {
                             if (i != range && j != range && k != range)
                                 continue;

                             _cache.EnsureLoaded(new PlanetIndex3(_center.Planet, _center.ChunkIndex.X + i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.ChunkIndex.X + i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.ChunkIndex.X - i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.ChunkIndex.X - i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.ChunkIndex.X + i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z - k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.ChunkIndex.X + i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z - k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.ChunkIndex.X - i, _center.ChunkIndex.Y + j, _center.ChunkIndex.Z - k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.ChunkIndex.X - i, _center.ChunkIndex.Y - j, _center.ChunkIndex.Z - k));
                             if (token.IsCancellationRequested) return;
                         }
        }
    }

    public interface IChunkLoader
    {
        /// <summary>
        /// Chunk index
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        void UpdatePosition(int planet, int i, int j, int k);
    }
}
