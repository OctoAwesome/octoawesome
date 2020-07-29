using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Information
{
    public abstract class BlockInteractionInformation : IPoolElement
    {
        public BlockInfo BlockInfo { get; }

        private IPool pool;

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
