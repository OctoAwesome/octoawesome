﻿using OctoAwesome.Basics.Properties;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class WoodBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Wood;

        public override string Icon => "wood_top";

        public override bool HasMetaData => true;

        public override string[] Textures { get; } = new[] { "wood_top", "wood_side" };

        public override IMaterialDefinition Material { get; }

        public WoodBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

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
