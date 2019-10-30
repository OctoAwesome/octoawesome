using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database
{
    class Value
    {
        private byte[] buffer;

        public Value(byte[] buffer, Key key)
        {
            this.buffer = buffer;
            Key = key;
        }

        public Key Key { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Content { get; set; }

        internal byte[] ToArray() => throw new NotImplementedException();
    }
}
