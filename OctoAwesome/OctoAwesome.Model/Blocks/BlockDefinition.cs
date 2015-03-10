using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class BlockDefinitionAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
