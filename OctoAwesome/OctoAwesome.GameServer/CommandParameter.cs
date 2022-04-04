namespace OctoAwesome.GameServer
{

    public struct CommandParameter
    {

        public uint ClientId { get; }
        public byte[] Data { get; }
        public CommandParameter(uint clientId, byte[] data)
        {
            ClientId = clientId;
            Data = data;
        }
    }
}
