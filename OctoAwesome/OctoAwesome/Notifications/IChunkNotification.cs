namespace OctoAwesome.Notifications
{

    public interface IChunkNotification
    {

        Index3 ChunkPos { get; }
        int Planet { get; }
    }
}
