namespace OctoAwesome.Database
{
    /// <summary>
    /// Represents a database value.
    /// </summary>
    public readonly struct Value
    {
        /// <summary>
        /// Gets the raw byte content of the database value.
        /// </summary>
        public byte[] Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> struct.
        /// </summary>
        /// <param name="buffer">The raw byte content of the database value.</param>
        public Value(byte[] buffer)
        {
            Content = buffer;
        }
    }
}
