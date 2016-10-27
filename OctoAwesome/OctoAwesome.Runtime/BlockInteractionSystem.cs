using OctoAwesome.Ecs;
using OctoAwesome.EntityComponents;
using OctoAwesome.GameEvents;

namespace OctoAwesome.Runtime
{
    [SystemConfiguration(After = new [] { "LookMovementSystem" })]
    public class BlockInteractionSystem : BaseSystemR2<PositionComponent, BlockInteractor>
    {
        public BlockInteractionSystem(EntityManager manager) : base(manager) {}

        protected override void Update(Entity e, PositionComponent r1, BlockInteractor r2)
        {
            if(!r2.Interact)
                return;

            ushort lastBlock = r1.LocalChunkCache.GetBlock(r2.Target);
            r1.LocalChunkCache.SetBlock(r2.Target, 0);

            if (lastBlock != 0)
            {
                var blockDefinition = DefinitionManager.Instance.GetBlockDefinitionByIndex(lastBlock);
                Manager.Publish(e, new PickupBlock {
                    Definition = blockDefinition,
                    Amount = 125
                });
            }
            r2.Interact = false;
        }
    }
}