﻿using OctoAwesome.Components;
using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OctoAwesome.Extension;

namespace OctoAwesome.Serialization.Entities
{
    /// <summary>
    /// Component container definition for <see cref="ComponentContainer{TComponent}"/> instances.
    /// </summary>
    /// <typeparam name="TComponent">The type of the components contained in the container definition.</typeparam>
    public sealed class ComponentContainerDefinition<TComponent> : ISerializable where TComponent : IComponent
    {
        private IEnumerable<Type>? components;
        private Type? type;

        /// <summary>
        /// Gets or sets the type of the <see cref="ComponentContainer{TComponent}"/> of this container definition.
        /// </summary>
        public Type Type
        {
            get => NullabilityHelper.NotNullAssert(type, $"{nameof(Type)} was not initialized!");
            set => type = NullabilityHelper.NotNullAssert(value, $"{nameof(Type)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the id of this container definition.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the number of components contained in this container definition.
        /// </summary>
        public int ComponentsCount { get; set; }

        /// <summary>
        /// Gets or sets the component types contained in this container definition.
        /// </summary>
        public IEnumerable<Type> Components
        {
            get => NullabilityHelper.NotNullAssert(components, $"{nameof(Components)} was not initialized!");
            set => components = NullabilityHelper.NotNullAssert(value, $"{nameof(Components)} cannot be initialized with null!");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainerDefinition{TContainer}"/> class.
        /// </summary>
        public ComponentContainerDefinition()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainerDefinition{TContainer}"/> class.
        /// </summary>
        /// <param name="entity">The component container this definition is for.</param>
        public ComponentContainerDefinition(ComponentContainer<TComponent> entity)
        {
            Type = entity.GetType();
            Id = entity.Id;
            var tmpComponents = entity.Components.ToList();
            ComponentsCount = tmpComponents.Count;
            Components = tmpComponents.Select(c => c.GetType());
        }

        /// <inheritdoc />
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Type.AssemblyQualifiedName!);
            writer.Write(Id.ToByteArray());
            writer.Write(ComponentsCount);

            foreach (var component in Components)
                writer.Write(component.AssemblyQualifiedName!);
        }

        /// <inheritdoc />
        public void Deserialize(BinaryReader reader)
        {
            var deserializedType = Type.GetType(reader.ReadString());
            Debug.Assert(deserializedType != null, nameof(deserializedType) + " != null");
            Type = deserializedType;
            Id = new Guid(reader.ReadBytes(16));
            ComponentsCount = reader.ReadInt32();
            var list = new List<Type>();

            for (int i = 0; i < ComponentsCount; i++)
            {
                var type = Type.GetType(reader.ReadString());
                Debug.Assert(type != null, nameof(type) + " != null");
                list.Add(type);
            }

            Components = list;
        }

        /// <summary>
        /// Database context for <see cref="ComponentContainerDefinition{TComponent}"/> instances.
        /// </summary>
        public sealed class ComponentContainerDefinitionContext : SerializableDatabaseContext<GuidTag<ComponentContainerDefinition<TComponent>>, ComponentContainerDefinition<TComponent>>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ComponentContainerDefinitionContext"/> class.
            /// </summary>
            /// <param name="database">The underlying database for this context.</param>
            public ComponentContainerDefinitionContext(Database<GuidTag<ComponentContainerDefinition<TComponent>>> database) : base(database)
            {
            }

            /// <inheritdoc />
            public override void AddOrUpdate(ComponentContainerDefinition<TComponent> value)
                => InternalAddOrUpdate(new GuidTag<ComponentContainerDefinition<TComponent>>(value.Id), value);

            /// <summary>
            /// Get all key tags in this database context.
            /// </summary>
            /// <returns>The <see cref="GuidTag{T}"/> identifying keys.</returns>
            public IEnumerable<GuidTag<ComponentContainerDefinition<TComponent>>> GetAllKeys() => Database.Keys;

            /// <inheritdoc />
            public override void Remove(ComponentContainerDefinition<TComponent> value)
                => InternalRemove(new GuidTag<ComponentContainerDefinition<TComponent>>(value.Id));
        }
    }
}
