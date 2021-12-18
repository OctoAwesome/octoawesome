namespace OctoAwesome.GameServer
{
    /// <summary>
    /// Struct for command parameters.
    /// </summary>
    public struct CommandParameter
    {
        /// <summary>
        /// Gets the id of the client the parameter was received from.
        /// </summary>
        public uint ClientId { get; }

        /// <summary>
        /// Gets the raw byte data for the parameter.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParameter"/> struct.
        /// </summary>
        /// <param name="clientId">The id of the client the parameter was received from.</param>
        /// <param name="data">The raw byte data for the parameter.</param>
        public CommandParameter(uint clientId, byte[] data)
        {
            ClientId = clientId;
            Data = data;
        }
    }
}
