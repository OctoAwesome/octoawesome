using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class ChunkLoader : IChunkLoader
    {
        private readonly IChunkCache _cache;
        private readonly int _maxRange;
        private Index3 _center;
        private Task _loadingTask;
        private CancellationTokenSource _cancellationToken;

        public ChunkLoader(IChunkCache cache, int range, Index3 center)
        {
            _center = center;
            _maxRange = range;
            _cache = cache;
        }

        public void UpdatePosition(int i, int j, int k)
        {
            _center = new Index3(_center.X + i, _center.Y + j, _center.Z + k);

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

                        _cache.Release(_center.X + i, _center.Y + j, _center.Z + k);
                        _cache.Release(_center.X + i, _center.Y - j, _center.Z + k);
                        _cache.Release(_center.X - i, _center.Y + j, _center.Z + k);
                        _cache.Release(_center.X - i, _center.Y - j, _center.Z + k);
                        _cache.Release(_center.X + i, _center.Y + j, _center.Z - k);
                        _cache.Release(_center.X + i, _center.Y - j, _center.Z - k);
                        _cache.Release(_center.X - i, _center.Y + j, _center.Z - k);
                        _cache.Release(_center.X - i, _center.Y - j, _center.Z - k);
                    }

            for (int range = 1; range < _maxRange; range ++) 
                 for (int i = 0; i <= range; i++) 
                     for (int j = 0; j <= range; j++)
                         for (int k = 0; k < range; k++)
                         {
                             if (i != range && j != range && k != range)
                                 continue;

                             _cache.EnsureLoaded(new Index3(_center.X + i, _center.Y + j, _center.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.X + i, _center.Y - j, _center.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.X - i, _center.Y + j, _center.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.X - i, _center.Y - j, _center.Z + k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.X + i, _center.Y + j, _center.Z - k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.X + i, _center.Y - j, _center.Z - k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.X - i, _center.Y + j, _center.Z - k));
                             if (token.IsCancellationRequested) return;

                             _cache.EnsureLoaded(new Index3(_center.X - i, _center.Y - j, _center.Z - k));
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
        void UpdatePosition(int i, int j, int k);
    }
}
