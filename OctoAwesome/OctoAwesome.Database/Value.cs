namespace OctoAwesome.Database
{
    public readonly struct Value
    {

        public byte[] Content { get; }

        public Value(byte[] buffer)
        {
            Content = buffer;
        }
    }
}
