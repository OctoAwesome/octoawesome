using engenious;
using engenious.Content.Serialization;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Graphs;

using OpenTK.Graphics.ES30;

using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for signaler blocks.
    /// </summary>
    public class PressurePlateBlockDefinition : BlockDefinition, INetworkBlock<Signal>
    {
        /// <inheritdoc />
        public override string Icon => "wood_side";

        /// <inheritdoc />
        public override string DisplayName => "PressurePlate Block";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["wood_top", "wood_side"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string[] TransferTypes { get; } = ["Signal"];

        /// <summary>
        /// Initializes a new instance of the <see cref="PressurePlateBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this signal cable block definition.</param>
        public PressurePlateBlockDefinition(SimpleBlockMaterialDefinition material)
        {
            Material = material;
        }


        public NodeBase CreateNode()
        {
            return new PressurePlateBlockNode();
        }
    }

    internal partial class PressurePlateBlockNode : Node<Signal>, ISourceNode<Signal>
    {
        public bool IsOn { get; set; } = false;

        public int Priority { get; } = 1;

        public string Channel { get; set; } = "Green";


        public SourceInfo<Signal> GetCapacity(Simulation simulation)
        {
            var positions = simulation.GlobalComponentList.GetAll<PositionComponent>();
            bool isOn = false;


            if (positions is not null)
            {
                foreach (var item in positions)
                {
                    if (item.Planet.Id != PlanetId)
                        continue;
                    var bc = item.Parent.GetComponent<BodyComponent>();
                    if (bc is null)
                        continue;
                    
                    var entityPos = item.Position.GlobalBlockIndex;

                    if (Position.Z == entityPos.Z - 1)
                    {
                        //var radius = bc.Radius;
                        //var halfRadius = radius / 2;

                        //var playerV = new Vector2(Position.X, Position.Y);
                        //var entityV = new Vector2(entityPos.X, entityPos.Y);

                        //var distanceV = Vector2.Distance
                        //var distance = entityPos.ShortestDistanceXY(Position.XY, item.Planet.Size.XY);

                        //if (Math.Abs(distance.X) < halfRadius + 0.5 && Math.Abs(distance.Y) < halfRadius + 0.5)
                        if(Position.XY == entityPos.XY)
                        {
                            isOn = true;
                            break;
                        }
                    }
                }
            }

            return new SourceInfo<Signal>(this, new Signal { Channel = Channel, Enabled = isOn });
        }

        public void Use(SourceInfo<Signal> targetInfo, IChunkColumn? column)
        {
        }
    }
}
