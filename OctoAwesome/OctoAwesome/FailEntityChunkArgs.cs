namespace OctoAwesome
{
    public class FailEntityChunkArgs
    {
        public Index2 CurrentChunk { get; set; }
        public IPlanet CurrentPlanet { get; set; }

        public Index2 TargetChunk { get; set; }
        public IPlanet TargetPlanet { get; set; }

        public Entity Entity { get; set; }
    }
}
