﻿using dotVariant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.SumTypes
{
    /// <summary>
    /// Selection variant of either <see cref="BlockInfo"/>, <see cref="Entity"/>.
    /// </summary>
    [Variant]
    public partial class Selection
    {
        static partial void VariantOf(BlockInfo blockinfo, ComponentContainer entity);
    }
}
