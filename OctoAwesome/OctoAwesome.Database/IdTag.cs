using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database
{
    public readonly struct IdTag : ITagable
    {
        public int Tag { get; }

        public IdTag(int id)
        {
            Tag = id;
        }
    }
}
