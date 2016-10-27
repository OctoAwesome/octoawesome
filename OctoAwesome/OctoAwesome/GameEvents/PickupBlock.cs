using OctoAwesome.Ecs;

namespace OctoAwesome.GameEvents
{
    public class PickupBlock : GameEvent
    {
        public IBlockDefinition Definition;
        public int Amount;
    }
}