using System;

namespace OctoAwesome.Database.Checks
{
    /// <summary>
    /// Exception thrown when a key in a <see cref="KeyStore{TTag}"/> is invalid.
    /// </summary>
    [Serializable]
    public sealed class InvalidKeyException : Exception
    {
        /// <summary>
        /// Gets the stream position the exception occured at.
        /// </summary>
        public long Position { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidKeyException"/> class.
        /// </summary>
        /// <param name="message">The error message for the exception.</param>
        /// <param name="position">The stream position the exception occured at.</param>
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
