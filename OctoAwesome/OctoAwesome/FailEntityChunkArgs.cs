namespace OctoAwesome
{
    public class FailEntityChunkArgs
    {
        public Index2 CurrentChunk { get; set; }
        public int CurrentPlanet { get; set; }

        public Index2 TargetChunk { get; set; }
        public int TargetPlanet { get; set; }

        public Entity Entity { get; set; }
    }
}
