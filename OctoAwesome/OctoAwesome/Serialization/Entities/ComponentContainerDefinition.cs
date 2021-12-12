using OctoAwesome.Components;
using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class ComponentContainerDefinition<TComponent> : ISerializable where TComponent : IComponent
    {

        public Type Type { get;  set; }
        

        public Guid Id { get;  set; }
        public int ComponentsCount { get;  set; }
        public IEnumerable<Type> Components { get;  set; }
        public ComponentContainerDefinition()
        {

        }

        public ComponentContainerDefinition(ComponentContainer<TComponent> entity)
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
        public sealed class ComponentContainerDefinitionContext : SerializableDatabaseContext<GuidTag<ComponentContainerDefinition<TComponent>>, ComponentContainerDefinition<TComponent>>
        {
            public ComponentContainerDefinitionContext(Database<GuidTag<ComponentContainerDefinition<TComponent>>> database) : base(database)
            {
            }
            public override void AddOrUpdate(ComponentContainerDefinition<TComponent> value)
                => InternalAddOrUpdate(new GuidTag<ComponentContainerDefinition<TComponent>>(value.Id), value);

            public IEnumerable<GuidTag<ComponentContainerDefinition<TComponent>>> GetAllKeys() => Database.Keys;
            public override void Remove(ComponentContainerDefinition<TComponent> value)
                => InternalRemove(new GuidTag<ComponentContainerDefinition<TComponent>>(value.Id));
        }
    }
}
