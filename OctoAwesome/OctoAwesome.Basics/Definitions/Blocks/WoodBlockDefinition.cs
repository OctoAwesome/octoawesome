using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;
using OctoAwesome.Chunking;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for wood log blocks.
    /// </summary>
    public sealed class WoodBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => OctoBasics.Wood;

        /// <inheritdoc />
        public override string Icon => "wood_top";

        /// <inheritdoc />
        public override bool HasMetaData => true;

        /// <inheritdoc />
        public override string[] Textures { get; init; } = ["wood_top", "wood_side"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WoodBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this wood block definition.</param>
        public WoodBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

        /// <inheritdoc />
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (wall)
            {
                case Wall.Top:
                case Wall.Bottom:
                    {
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
                                return 0;
                        }
                    }

                case Wall.Back: // North
                case Wall.Front: // South
                    {
                        switch (orientation)
                        {
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                                return 0;
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                return 1;
                        }
                    }

                case Wall.Left:
                case Wall.Right:
                    {
                        switch (orientation)
                        {
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
                                return 0;
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                            case OrientationFlags.SideBottom:
                            case OrientationFlags.SideTop:
                            default:
                                return 1;
                        }
                    }
            }

            // Assert this
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
                    switch (orientation)//top and bottom north south
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
                    switch (orientation) //east west
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
    }
}
