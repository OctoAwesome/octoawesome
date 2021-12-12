using OctoAwesome.Definitions;
using OctoAwesome.Pooling;
using System;

namespace OctoAwesome.Services
{

    public sealed class BlockVolumeState : IPoolElement
    {

        public BlockInfo BlockInfo { get; private set; }
        public IBlockDefinition BlockDefinition { get; private set; }
        public decimal VolumeRemaining { get; internal set; }

        public DateTimeOffset ValidUntil { get; set; }

        private IPool pool;

        public void Initialize(BlockInfo info, IBlockDefinition blockDefinition, DateTimeOffset validUntil)
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
            pool.Return(this);
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
