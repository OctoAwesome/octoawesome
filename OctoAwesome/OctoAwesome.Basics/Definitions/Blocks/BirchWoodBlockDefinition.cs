using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Properties;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Information;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for birch wood logs.
    /// </summary>
    public sealed class BirchWoodBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.BirchWood;

        /// <inheritdoc />
        public override string Icon => "birch_wood_top";

        /// <inheritdoc />
        public override bool HasMetaData => true;

        /// <inheritdoc />
        public override string[] Textures { get; } = [ "birch_wood_top",
                                                      "birch_wood_side" ];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BirchWoodBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this wood block definition.</param>
        public BirchWoodBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

        /// <inheritdoc />
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager,
            int x, int y, int z)
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

                case Wall.Front:
                case Wall.Back:

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
