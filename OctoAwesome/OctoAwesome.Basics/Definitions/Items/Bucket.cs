﻿using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Items
{
    class Bucket : Item
    {
        public Bucket(BucketDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
