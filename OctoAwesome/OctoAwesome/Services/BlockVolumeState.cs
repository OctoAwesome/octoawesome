using OctoAwesome.Definitions;
using OctoAwesome.Information;
using OctoAwesome.Pooling;

using System;

namespace OctoAwesome.Services
{
    public sealed class BlockVolumeState : IPoolElement
    {
        public IBlockInteraction BlockInfo { get; set; }
        public IBlockDefinition BlockDefinition { get; set; }
        public decimal VolumeRemaining { get; internal set; }
        public DateTimeOffset ValidUntil { get; set; }

        private IPool pool;

        public void Initialize(IBlockInteraction info, IBlockDefinition blockDefinition, DateTimeOffset validUntil)
        {
            BlockInfo = info;
            BlockDefinition = blockDefinition;
            VolumeRemaining = blockDefinition.VolumePerUnit;
            ValidUntil = validUntil;
        }

        public void Init(IPool pool)
        {
            this.pool = pool;
        }

        public void Release()
        {
            pool.Push(this);
        }

        internal bool TryReset()
        {
            if (ValidUntil >= DateTimeOffset.Now)
                return false;

            VolumeRemaining = BlockDefinition.VolumePerUnit;
            return true;
        }

        internal void RestoreTime()
        {
            ValidUntil = DateTimeOffset.Now.Add(BlockDefinition.TimeToVolumeReset);
        }
    }
}
