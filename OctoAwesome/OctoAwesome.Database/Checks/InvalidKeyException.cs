using System;

namespace OctoAwesome.Database.Checks
{
    [Serializable]
    public sealed class InvalidKeyException : Exception
    {
        public long Position { get;  }

        public InvalidKeyException(string message, long position) : base($"{message} on Position {position}")
        {
            Position = position;
            Data.Add(nameof(Position), position);
        }

        private InvalidKeyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
