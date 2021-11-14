﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public interface IEntityList : ICollection<Entity>
    {
        IEnumerable<FailEntityChunkArgs> FailChunkEntity();
    }
}
