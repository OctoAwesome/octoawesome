using OctoAwesome.Components;
using OctoAwesome.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using OctoAwesome.Caching;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all component based entities.
    /// </summary>
    /// <typeparam name="T">Type of the components contained in the list.</typeparam>
    public class ComponentList<T> : IEnumerable<T> where T : IComponent, ISerializable
    {
        private readonly Action<T>? insertValidator;
        private readonly Action<T>? removeValidator;
        private readonly Action<T>? onInserter;
        private readonly Action<T>? onRemover;

        private readonly Dictionary<Type, T> components = new Dictionary<Type, T>();

        /// <summary>
        /// Gets a component of a specific type; or <c>null</c> if no matching component is found.
        /// </summary>
        /// <param name="type">The type to get the component for</param>
        public T? this[Type type] => components.TryGetValue(type, out var result) ? result : default;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentList{T}"/> class.
        /// </summary>
        public ComponentList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentList{T}"/> class.
        /// </summary>
        /// <param name="insertValidator">The validator for insertions.</param>
        /// <param name="removeValidator">The validator for removals.</param>
        /// <param name="onInserter">The method to call on insertion.</param>
        /// <param name="onRemover">The method to call on removal.</param>
        public ComponentList(Action<T>? insertValidator, Action<T>? removeValidator, Action<T>? onInserter, Action<T>? onRemover)
        {
            this.insertValidator = insertValidator;
            this.removeValidator = removeValidator;
            this.onInserter = onInserter;
            this.onRemover = onRemover;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
            => components.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => components.Values.GetEnumerator();

        /// <summary>
        /// Adds a new Component to the List.
        /// </summary>
        /// <param name="component">The component to add to the list.</param>
        /// <typeparam name="V">The type of the component to add.</typeparam>
        public void AddComponent<V>(V component) where V : T
            => AddComponent(component, false);


        /// <summary>
        /// Adds or replaces a component.
        /// </summary>
        /// <param name="component">The component to add or replace.</param>
        /// <param name="replace">
        /// Whether to add the component when a component with the same type already exists in the list, or not add it if it does.
        /// </param>
        /// <typeparam name="V">The type of the component to add.</typeparam>
        public void AddComponent<V>(V component, bool replace) where V : T
        {
            Type type = component.GetType();

            if (components.ContainsKey(type))
            {
                if (replace)
                {
                    RemoveComponent<V>();
                }
                else
                {
                    return;
                }
            }

            insertValidator?.Invoke(component);
            components.Add(type, component);
            onInserter?.Invoke(component);
        }

        /// <summary>
        /// Checks whether the component of <typeparamref name="V"/> is present in the internal dictionary as a key.
        /// </summary>
        /// <typeparam name="V">The type to search in the internal dictionary</typeparam>
        /// <returns>
        /// <list type="bullet">
        ///     <item><see langword="true"/> if the component was found</item>
        ///     <item><see langword="false"/> if the component was not found</item>
        /// </list>
        /// </returns>
        public bool ContainsComponent<V>()
        {
            var type = typeof(V);
            if (type.IsAbstract || type.IsInterface)
            {
                return components.Any(x => type.IsAssignableFrom(x.Key));
            }
            return components.ContainsKey(type);
        }

        /// <summary>
        /// Tries to return the Component of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>True if the component was found, false otherwise</returns>
        public bool TryGetComponent<V>([MaybeNullWhen(false)] out V component) where V : T
        {
            var contains = components.TryGetValue(typeof(V), out var result);
            component = default;
            if (!contains)
                return contains;

            component = GenericCaster<T, V>.Cast(result!);

            return contains;
        }

        /// <summary>
        /// Returns the Component of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>Component</returns>
        public V? GetComponent<V>()
        {
            if (components.TryGetValue(typeof(V), out var result))
                return (V)(object)result;

            return default;
        }

        /// <summary>
        /// Removes the Component of the given type.
        /// </summary>
        /// <typeparam name="V">The type of the component to remove.</typeparam>
        /// <returns>A value indicating whether the remove was successful or not.</returns>
        public bool RemoveComponent<V>() where V : T
        {
            if (!components.TryGetValue(typeof(V), out var component))
                return false;

            removeValidator?.Invoke(component);
            if (components.Remove(typeof(V)))
            {
                onRemover?.Invoke(component);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Serializes the component list to a binary writer..
        /// </summary>
        /// <param name="writer">The binary writer to serialize the component list to.</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(components.Count);
            foreach (var componente in components)
            {
                writer.Write(componente.Key.AssemblyQualifiedName!);
                componente.Value.Serialize(writer);

            }
        }

        /// <summary>
        /// Deserializes the component list from a binary reader..
        /// </summary>
        /// <param name="reader">The binary reader to deserialize the component list from.</param>
        public virtual void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();

                var type = Type.GetType(name);

                Debug.Assert(type != null, nameof(type) + " != null");
                if (!components.TryGetValue(type, out var component))
                {
                    component = (T)TypeContainer.GetUnregistered(type);
                    //components.Add(type, component);
                    AddComponent(component);
                }

                component.Deserialize(reader);
            }
        }
    }
}
