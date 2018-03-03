using engenious;
using OctoAwesome.Entities;
using OctoAwesome.Common;
using System;
using System.Linq;
namespace OctoAwesome.Runtime
{
    // sealed -> prevent abuse of third party´s
    /// <summary>
    /// Diese Berechnungen sollten nicht der Extension überlassen werden.
    /// </summary>
    public sealed class GameService : IGameService
    {
        /// <summary>
        /// <see cref="IDefinitionManager"/>
        /// </summary>
        public IDefinitionManager DefinitionManager { get; }
        private const float GAP = 0.01f;
        /// <summary>
        /// Default Construktor.
        /// </summary>
        /// <param name="manager"></param>
        public GameService(IDefinitionManager manager)
        {
            DefinitionManager = manager;
        }
        /// <summary>
        /// Take a Block from the World.
        /// </summary>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="block"><see cref="IInventory"/> of the <see cref="Entity"/></param>
        public bool TakeBlock(IEntityController controller, ILocalChunkCache cache, out IInventoryableDefinition block)
        {
            block = null;
            if (controller == null || cache == null) return false;
            if (controller.InteractInput && controller.SelectedBlock.HasValue)
            {
                ushort lastBlock = cache.GetBlock(controller.SelectedBlock.Value, true);
                if (lastBlock != 0)
                {
                    block = DefinitionManager.GetDefinitionByIndex<IInventoryableDefinition>(lastBlock);
                }
                controller.InteractInput = false;
            }
            return block != null;
        }
        /// <summary>
        /// Take a Block from the World and add it to the invetory.
        /// </summary>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="inventory">Returned <see cref="IInventoryableDefinition"/> on success.</param>
        public bool TakeBlock(IEntityController controller, ILocalChunkCache cache, IInventory inventory)
        {
            if (controller == null || cache == null || inventory == null) return false;
            if (controller.InteractInput && controller.SelectedBlock.HasValue)
            {
                ushort lastBlock = cache.GetBlock(controller.SelectedBlock.Value, true);
                if (lastBlock != 0)
                {
                    var blockDefinition = DefinitionManager.GetDefinitionByIndex<IInventoryableDefinition>(lastBlock);
                    if (blockDefinition != null)
                        inventory.AddUnit(blockDefinition);

                }
                controller.InteractInput = false;
            }
            return true;
        }
        /// <summary>
        /// Set a block or interact with a Tool.
        /// </summary>
        /// <param name="position"><see cref="Coordinate"/> of the <see cref="Entity"/></param>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="slot">Used <see cref="InventorySlot"/></param>
        /// <param name="inventory"><see cref="IInventory"/> of the <see cref="Entity"/></param>
        /// <param name="height">Height of the <see cref="Entity"/></param>
        /// <param name="radius">Radius of the <see cref="Entity"/></param>
        /// <returns></returns>
        public bool InteractBlock(Coordinate position, float radius, float height, IEntityController controller,
            ILocalChunkCache cache, InventorySlot slot, IInventory inventory)
        {
            //TODO: Cheats vermeiden
            // durch diese methode ist nicht sichergestellt das der block aus dem inventar entfernt wird...
            // model veranlassen das inventar zu mieten.
            // -> public IInventory RentInventory(Enttiy entity);
            // -> if(!rentedinvetory.Contains(inventory)) return;
            if (controller == null || cache == null || slot == null || inventory == null)
            {
                controller.ApplyInput = false;
                return false;
            }
            bool result = false;
            if (controller.ApplyInput && controller.SelectedBlock.HasValue)
            {
                Index3 add;
                switch (controller.SelectedSide)
                {
                    case OrientationFlags.SideWest: add = new Index3(-1, 0, 0); break;
                    case OrientationFlags.SideEast: add = new Index3(1, 0, 0); break;
                    case OrientationFlags.SideSouth: add = new Index3(0, -1, 0); break;
                    case OrientationFlags.SideNorth: add = new Index3(0, 1, 0); break;
                    case OrientationFlags.SideBottom: add = new Index3(0, 0, -1); break;
                    case OrientationFlags.SideTop: add = new Index3(0, 0, 1); break;
                    default: add = new Index3(); break;
                }
                if (slot.Definition is IBlockDefinition blockdefinition)
                {
                    Index3 idx = controller.SelectedBlock.Value + add;
                    var boxes = blockdefinition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);
                    bool intersects = false;
                    
                    if(radius != 0 || height != 0)
                    {
                        var playerBox = new BoundingBox(
                           new Vector3(
                              position.GlobalBlockIndex.X + position.BlockPosition.X - radius + GAP,
                              position.GlobalBlockIndex.Y + position.BlockPosition.Y - radius + GAP,
                              position.GlobalBlockIndex.Z + position.BlockPosition.Z + GAP),
                           new Vector3(
                              position.GlobalBlockIndex.X + position.BlockPosition.X + radius - GAP,
                              position.GlobalBlockIndex.Y + position.BlockPosition.Y + radius - GAP,
                              position.GlobalBlockIndex.Z + position.BlockPosition.Z + height - GAP)
                           );
                        // Nicht in sich selbst reinbauen
                        foreach (var box in boxes)
                        {
                            var newBox = new BoundingBox(idx + box.Min, idx + box.Max);
                            if (newBox.Min.X < playerBox.Max.X && newBox.Max.X > playerBox.Min.X &&
                                newBox.Min.Y < playerBox.Max.Y && newBox.Max.X > playerBox.Min.Y &&
                                newBox.Min.Z < playerBox.Max.Z && newBox.Max.X > playerBox.Min.Z)
                                intersects = true;
                        }
                    }

                    if (!intersects && inventory.RemoveUnit(slot))
                    {
                        cache.SetBlock(idx, DefinitionManager.GetDefinitionIndex(blockdefinition));
                        result = true;
                    }
                }
                else throw new NotImplementedException("slot definition is not a IBlockDefinition");
                controller.ApplyInput = false;
            }
            return result;
        }
        /// <summary>
        /// Calculates the collison between an <see cref="Entity"/> and the world.
        /// </summary>
        /// <param name="gameTime">Simulation time</param>
        /// <param name="entity">Entity</param>
        /// <param name="radius">radius of the <see cref="Entity"/></param>
        /// <param name="height">height of the <see cref="Entity"/></param>
        /// <param name="deltaPosition">Calculated delta position for this cycle</param>
        /// <param name="velocity">claculated velocity for this cycle</param>
        /// <returns>the velocity of the <see cref="Entity"/> after collision checks</returns>
        public Vector3 WorldCollision(GameTime gameTime, Entity entity, float radius, float height, 
            Vector3 deltaPosition, Vector3 velocity)
        {
            Coordinate position = entity.Position;
            Vector3 move = deltaPosition;

            //Blocks finden die eine Kollision verursachen könnten
            int minx = (int) Math.Floor(Math.Min(
                position.BlockPosition.X - radius,
                position.BlockPosition.X - radius + deltaPosition.X));
            int maxx = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.X + radius,
                position.BlockPosition.X + radius + deltaPosition.X));
            int miny = (int) Math.Floor(Math.Min(
                position.BlockPosition.Y - radius,
                position.BlockPosition.Y - radius + deltaPosition.Y));
            int maxy = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Y + radius,
                position.BlockPosition.Y + radius + deltaPosition.Y));
            int minz = (int) Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + deltaPosition.Z));
            int maxz = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Z + height,
                position.BlockPosition.Z + height + deltaPosition.Z));

            //Beteiligte Flächen des Spielers
            var playerplanes = CollisionPlane.GetEntityCollisionPlanes(radius, height, velocity, position);

            bool abort = false;

            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        move = velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = entity.Cache.GetBlock(blockPos);
                        if (block == 0) continue;

                        var blockplane = CollisionPlane.GetBlockCollisionPlanes(pos, velocity);

                        var planes = from pp in playerplanes
                                     from bp in blockplane
                                     where CollisionPlane.Intersect(bp, pp)
                                     let distance = CollisionPlane.GetDistance(bp, pp)
                                     where CollisionPlane.CheckDistance(distance, move)
                                     select new { BlockPlane = bp, PlayerPlane = pp, Distance = distance };

                        foreach (var plane in planes)
                        {

                            var subvelocity = (plane.Distance / (float) gameTime.ElapsedGameTime.TotalSeconds);
                            var diff = velocity - subvelocity;

                            float vx;
                            float vy;
                            float vz;

                            if (plane.BlockPlane.normal.X != 0 && (velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 ||
                                velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                vx = subvelocity.X;
                            else
                                vx = velocity.X;

                            if (plane.BlockPlane.normal.Y != 0 && (velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 ||
                                velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                vy = subvelocity.Y;
                            else
                                vy = velocity.Y;

                            if (plane.BlockPlane.normal.Z != 0 && (velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 ||
                                velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                vz = subvelocity.Z;
                            else
                                vz = velocity.Z;

                            velocity = new Vector3(vx, vy, vz);

                            if (vx == 0 && vy == 0 && vz == 0)
                            {
                                abort = true;
                                break;
                            }
                        }
                    }
                }
            }
            return velocity;
        }
    }
}
