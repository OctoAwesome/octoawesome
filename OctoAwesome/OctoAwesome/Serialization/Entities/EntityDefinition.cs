using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDefinition : ISerializable
    {
        public Type Type { get;  set; }
        public Guid Id { get;  set; }
        public int ComponentsCount { get;  set; }
        public IEnumerable<Type> Components { get;  set; }

        public EntityDefinition()
        {

        }
        public EntityDefinition(Entity entity)
        {
            Type = entity.GetType();
            Id = entity.Id;
            var tmpComponents = entity.Components.ToList();
            ComponentsCount = tmpComponents.Count;
            Components = tmpComponents.Select(c => c.GetType());
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Type.AssemblyQualifiedName!);
            writer.Write(Id.ToByteArray());
            writer.Write(ComponentsCount);

            foreach (var component in Components)
                writer.Write(component.AssemblyQualifiedName!);
        }

        public void Deserialize(BinaryReader reader)
        {
            Type = Type.GetType(reader.ReadString());
            Id = new Guid(reader.ReadBytes(16));
            ComponentsCount = reader.ReadInt32();
            var list = new List<Type>();

            for (int i = 0; i < ComponentsCount; i++)
                list.Add(Type.GetType(reader.ReadString()));

            Components = list;
        }

        public sealed class EntityDefinitionContext : SerializableDatabaseContext<GuidTag<EntityDefinition>, EntityDefinition>
        {
            public EntityDefinitionContext(Database<GuidTag<EntityDefinition>> database) : base(database)
            {
            }

            public override void AddOrUpdate(EntityDefinition value)
                => InternalAddOrUpdate(new GuidTag<EntityDefinition>(value.Id), value);

            public IEnumerable<GuidTag<EntityDefinition>> GetAllKeys() => Database.Keys;

            public override void Remove(EntityDefinition value)
                => InternalRemove(new GuidTag<EntityDefinition>(value.Id));
        }
    }
}
