using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Information
{
    public abstract class BlockInteractionInformation : IPoolElement
    {
        public BlockInfo BlockInfo { get; protected set; }
        public IBlockDefinition BlockDefinition { get; protected set; }

        private IPool pool;

        public virtual void Initialize(BlockInfo info, IBlockDefinition blockDefinition)
        {
            BlockInfo = info;
            BlockDefinition = blockDefinition;
        }

        public virtual void Init(IPool pool)
        {
            this.pool = pool;
        }

        public virtual void Release()
        {
            pool.Push(this);
        }
    }
}
