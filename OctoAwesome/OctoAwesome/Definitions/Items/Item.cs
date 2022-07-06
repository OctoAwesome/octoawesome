using engenious;
using OctoAwesome.Serialization;
using System;
using System.IO;

namespace OctoAwesome.Definitions.Items
{
    /// <summary>
    /// Base class for items.
    /// </summary>
    public abstract class Item : IItem, IInventoryable, ISerializable
    {
        /// <inheritdoc />
        public int Condition { get; set; }

        /// <inheritdoc />
        public Coordinate? Position { get; set; }

        /// <inheritdoc />
        public IItemDefinition Definition { get; private set; }

        /// <inheritdoc />
        public IMaterialDefinition Material { get; private set; }

        /// <inheritdoc />
        public virtual int VolumePerUnit => 1;

        /// <inheritdoc />
        public virtual int StackLimit => 1;

        /// <inheritdoc />
        public int Density => Material.Density;

        private readonly IDefinitionManager definitionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="definition">The item definition.</param>
        /// <param name="material">The material definition.</param>
        public Item(IItemDefinition definition, IMaterialDefinition material)
        {
            Definition = definition;
            Material = material;
            Condition = 99;

            definitionManager = TypeContainer.Get<IDefinitionManager>();
        }

        /// <inheritdoc />
        public virtual int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            //TODO Condition calculation

            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material is ISolidMaterialDefinition solid)
            {
                if (solid.Granularity > 1)
                    return 0;
            }

            if (Material.Hardness * 1.2f < material.Hardness)
                return 0;

            //(Hardness Effectivity + Fracture Effectivity) / 2
            return ((Material.Hardness - material.Hardness) * 3 + 100) * volumePerHit / 100;
        }

        /// <inheritdoc />
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Definition.GetType().FullName!);
            writer.Write(Material.GetType().FullName!);

            InternalSerialize(writer);
        }

        /// <summary>
        /// Serializes <see cref="Condition"/>, and <see cref="Position"/> to a binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to serialize to.</param>
        protected void InternalSerialize(BinaryWriter writer)
        {
            writer.Write(Condition);
            writer.Write(Position.HasValue);
            if (Position.HasValue)
            {
                writer.Write(Position.Value.Planet);
                writer.Write(Position.Value.GlobalBlockIndex.X);
                writer.Write(Position.Value.GlobalBlockIndex.Y);
                writer.Write(Position.Value.GlobalBlockIndex.Z);
                writer.Write(Position.Value.BlockPosition.X);
                writer.Write(Position.Value.BlockPosition.Y);
                writer.Write(Position.Value.BlockPosition.Z);
            }
        }

        /// <inheritdoc />
        public virtual void Deserialize(BinaryReader reader)
        {
            Definition = definitionManager.GetDefinitionByTypeName<IItemDefinition>(reader.ReadString());
            Material = definitionManager.GetDefinitionByTypeName<IMaterialDefinition>(reader.ReadString());

            InternalDeserialize(reader);
        }

        /// <summary>
        /// Deserializes <see cref="Condition"/>, and <see cref="Position"/> from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader to deserialize from.</param>
        protected void InternalDeserialize(BinaryReader reader)
        {
            Condition = reader.ReadInt32();
            if (reader.ReadBoolean())
            {
                // Position
                int planet = reader.ReadInt32();
                int blockX = reader.ReadInt32();
                int blockY = reader.ReadInt32();
                int blockZ = reader.ReadInt32();
                float posX = reader.ReadSingle();
                float posY = reader.ReadSingle();
                float posZ = reader.ReadSingle();

                Position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));
            }
        }

        /// <summary>
        /// Deserializes an item of a given type from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader to deserialize the item from.</param>
        /// <param name="itemType">The type of the item to deserialize.</param>
        /// <param name="manager">
        /// The definition manager to use for resolving <see cref="Material"/> and <see cref="Definition"/>.
        /// </param>
        /// <returns>The deserialized item.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the created type is not of type <see cref="Item"/>.
        /// </exception>
        public static Item Deserialize(BinaryReader reader, Type itemType, IDefinitionManager manager)
        {
            var definition = manager.GetDefinitionByTypeName<IItemDefinition>(reader.ReadString());
            var material = manager.GetDefinitionByTypeName<IMaterialDefinition>(reader.ReadString());

            if (Activator.CreateInstance(itemType, definition, material) is not Item item)
                throw new ArgumentException($"Type of {itemType.Name} is not of type Item.");

            item.InternalDeserialize(reader);
            return item;
        }

        public IDefinition GetDefinition() => Definition;
    }
}
