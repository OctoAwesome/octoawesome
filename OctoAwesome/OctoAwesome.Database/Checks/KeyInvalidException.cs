using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database.Checks
{
    [Serializable]
    public class KeyInvalidException : Exception
    {
        public long Position { get;  }

        public KeyInvalidException(string message, long position) : base($"{message} on Position {position}")
        {
            Position = position;
            Data.Add(nameof(Position), position);
        }

        protected KeyInvalidException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
