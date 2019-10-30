using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database
{
    class Key
    {
        public const int KEY_SIZE = sizeof(long) + sizeof(int) + sizeof(int);

        public int Target { get; set; }
        public long Index { get; set; }
        public int Length { get; set; }
    }
}
