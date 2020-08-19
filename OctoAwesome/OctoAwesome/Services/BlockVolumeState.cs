using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Services
{
    public sealed class BlockVolumeState : IPoolElement
    {
        public BlockInfo BlockInfo { get; protected set; }
        public IBlockDefinition BlockDefinition { get; protected set; }
        public decimal VolumeRemaining { get; internal set; }

        private IPool pool;

        public void Initialize(BlockInfo info, IBlockDefinition blockDefinition)
        {
            BlockInfo = info;
            BlockDefinition = blockDefinition;
            VolumeRemaining = blockDefinition.VolumePerUnit;
        }

        public void Init(IPool pool)
        {
            this.pool = pool;
        }

        public void Release()
        {
            pool.Push(this);
        }
    }
}
