using engenious.Content.Serialization;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;

using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for cactus blocks.
    /// </summary>
    public class CactusBlockDefinition : BlockDefinition, INetworkBlock<int>, INetworkBlock<Signal>
    {
        /// <inheritdoc />
        public override string Icon => "cactus_inside";

        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Cactus;

        /// <inheritdoc />
        public override string[] Textures { get; } = ["cactus_inside", "cactus_side", "cactus_top"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string[] TransferTypes { get; } = ["Energy", "Signal"];

        /// <summary>
        /// Initializes a new instance of the <see cref="CactusBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cactus block definition.</param>
        public CactusBlockDefinition(CactusMaterialDefinition material)
        {
            Material = material;
        }

        /// <inheritdoc />
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager,
            int x, int y, int z)
        {
            var meta = manager.GetBlockMeta(x, y, z);
            OrientationFlags orientation = (OrientationFlags)(meta & 0xFF);
            var rotData = (meta >> 7) & 2;

            if (rotData > 0)
                return rotData;

            switch (wall)
            {
                case Wall.Top:
                    {
                        ushort topblock = manager.GetBlock(x, y, z + 1);

                        switch (orientation)
                        {
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                                return 1;
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
                        }
                    }
                case Wall.Bottom:
                    {
                        ushort topblock = manager.GetBlock(x, y, z + 1);

                        switch (orientation)
                        {
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                                return 1;
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
                        }
                    }

                case Wall.Front:
                    {
                        ushort topblock = manager.GetBlock(x, y, z + 1);

                        switch (orientation)
                        {
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                return 1;
                        }
                    }
                case Wall.Back:
                    {
                        ushort topblock = manager.GetBlock(x, y, z + 1);

                        switch (orientation)
                        {
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                return 1;
                        }
                    }

                case Wall.Left:
                    {
                        ushort topblock = manager.GetBlock(x, y, z + 1);

                        switch (orientation)
                        {
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                return 1;
                        }
                    }

                case Wall.Right:
                    {
                        ushort topblock = manager.GetBlock(x, y, z + 1);

                        switch (orientation)
                        {
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                return 1;
                        }
                    }
            }

            // Should never happen
            // Assert here
            return -1;
        }

        /// <inheritdoc />
        public override int GetTextureRotation(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {

            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (wall)
            {
                case Wall.Top:
                case Wall.Bottom:
                case Wall.Back:
                case Wall.Front:
                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                            return 1;
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 0;
                    }
                case Wall.Left:
                case Wall.Right:
                    switch (orientation)
                    {
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            return 1;
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 0;
                    }
                default:
                    return base.GetTextureRotation(wall, manager, x, y, z); //should never ever happen
            }
        }

        public NodeBase CreateNode()
        {
            return new CactusBlockNode();
        }

    }

    internal partial class CactusBlockNode : Node<int>, ISourceNode<int>, ITargetNode<Signal>
    {
        bool isOn = false;
        bool signalEnabled = false;

        public int Priority { get; } = 1;

        public override void Interact()
        {
            isOn = !isOn;
        }

        public SourceInfo<int> GetCapacity(Simulation simulation)
        {
            return new SourceInfo<int>(this, isOn ? 100 : 0);
        }

        public void Use(SourceInfo<int> targetInfo, IChunkColumn? column)
        {
            var oldMeta = column.GetBlockMeta(Position.X, Position.Y, Position.Z);

            var rotData = ((oldMeta >> 7) + 1) & 2;
            if (isOn && rotData == 0)
                rotData = 1;
            else if (!isOn)
                rotData = 0;

            oldMeta = ((isOn ? rotData : 0) << 7) | (oldMeta & 0xFF);

            column.SetBlockMeta(Position, oldMeta);
        }


        public void Execute(TargetInfo<Signal> targetInfo, IChunkColumn? column)
        {
            if(signalEnabled != targetInfo.Data.Enabled)
            {
                signalEnabled = !signalEnabled;
                isOn = signalEnabled;
            }
        }

        public TargetInfo<Signal> GetRequired()
        {
            return new TargetInfo<Signal>(this, new Signal { Channel = "Green" });
        }
    }
}
