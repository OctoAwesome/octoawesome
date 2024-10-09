namespace OctoAwesome.Definitions
{
    public class BurnableDefinition : IDefinition
    {
        /// <inheritdoc />
        public string DisplayName { get; init; }
        /// <inheritdoc />
        public string Icon { get; init; }

        public int EnergyPerPiece { get; init; }
    }
}
