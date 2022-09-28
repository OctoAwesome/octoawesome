using OctoAwesome.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using OctoAwesome.Caching;
using System.Diagnostics;

namespace OctoAwesome.Components;

/// <summary>
/// Base Class for all component based entities.
/// </summary>
/// <typeparam name="T">Type of the components contained in the list.</typeparam>
public class ComponentList<T> : IEnumerable<T> where T : IComponent, ISerializable
{

    /// <summary>
    /// Gets a component of a specific type; or <c>null</c> if no matching component is found.
    /// </summary>
    /// <param name="type">The type to get the component for</param>
    /// 
    public IList<T> this[Type type] => componentsByType.TryGetValue(type, out var result) ?
        result : Array.Empty<T>();

    private IReadOnlyCollection<Type> TypeKeys => componentsByType.Keys;

    private readonly Action<T> insertValidator;
    private readonly Action<T> removeValidator;
    private readonly Action<T> onInserter;
    private readonly Action<T> onRemover;

    private readonly HashSet<T> flatComponents = new();
    private readonly Dictionary<Type, List<T>> componentsByType = new();

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
        => flatComponents.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => flatComponents.GetEnumerator();

    /// <summary>
    /// Adds a component if the type is not already present.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <typeparam name="V">The type of the component to add.</typeparam>
    public void AddIfTypeNotExists<V>(V component) where V : T
    {
        Type type = component.GetType();

        if (flatComponents.Contains(component))
            return;

        if (!componentsByType.TryGetValue(type, out var existing))
        {
            existing = new();
            componentsByType[type] = existing;
        }
        else if (existing.Count > 0)
        {
            return;
        }

        existing.Add(component);
        flatComponents.Add(component);
        insertValidator?.Invoke(component);
        onInserter?.Invoke(component);
    }

    /// <summary>
    /// Adds a component.
    /// </summary>
    /// <param name="component">The component to add or replace.</param>
    /// <typeparam name="V">The type of the component to add.</typeparam>
    public void Add<V>(V component) where V : T
    {
        Type type = component.GetType();

        if (flatComponents.Contains(component))
            return;


        if (componentsByType.TryGetValue(type, out var existing))
        {
            existing.Add(component);
        }
        else
        {
            componentsByType[type] = new() { component };
        }

        flatComponents.Add(component);
        insertValidator?.Invoke(component);
        onInserter?.Invoke(component);
    }


    /// <summary>
    /// Adds a component.
    /// </summary>
    /// <param name="component">The component to add or replace.</param>
    /// <typeparam name="V">The type of the component to add.</typeparam>
    public void AddIfNotExists<V>(V component) where V : T
    {
        Type type = component.GetType();

        if (flatComponents.Contains(component))
            return;


        if (componentsByType.TryGetValue(type, out var existing))
        {
            if (!existing.Contains(component))
                existing.Add(component);

        }
        else
        {
            componentsByType[type] = new() { component };
        }

        flatComponents.Add(component);
        insertValidator?.Invoke(component);
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
    public bool Contains<V>()
    {
        var type = typeof(V);
        foreach (var x in TypeKeys)
        {
            if (type.IsAssignableFrom(x))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to return the Component of the given Type or null
    /// </summary>
    /// <typeparam name="V">Component Type</typeparam>
    /// <returns>True if the component was found, false otherwise</returns>
    public bool TryGet<V>([MaybeNullWhen(false)] out V component) where V : T
    {
        var contains = componentsByType.TryGetValue(typeof(V), out var result);
        if (!contains || result is null || result.Count < 1)
        {
            component = default;
            return false;
        }

        component = GenericCaster<T, V>.Cast(result[0]);
        return contains;
    }

    /// <summary>
    /// Returns the first Component of the given Type or null
    /// </summary>
    /// <typeparam name="V">Component Type</typeparam>
    /// <returns>Component</returns>
    public V? Get<V>()
    {
        if (componentsByType.TryGetValue(typeof(V), out var result) && result.Count > 0)
            return GenericCaster<T, V>.Cast(result[0]);

        return default;
    }

    /// <summary>
    /// Returns a list of the Component with the given Type or null
    /// </summary>
    /// <typeparam name="V">Component Type</typeparam>
    /// <returns>Component</returns>
    public List<V>? GetAll<V>()
    {
        if (componentsByType.TryGetValue(typeof(V), out var result))
            return GenericCaster<T, V>.CastList<List<T>, List<V>>(result);

        return default;
    }

    /// <summary>
    /// Removes the Component of the given type.
    /// </summary>
    /// <typeparam name="V">The type of the component to remove.</typeparam>
    /// <returns>A value indicating whether the remove was successful or not.</returns>
    public bool Remove<V>(V component) where V : T
    {
        if (!flatComponents.Contains(component))
            return false;

        removeValidator?.Invoke(component);
        if (flatComponents.Remove(component))
        {
            onRemover?.Invoke(component);

            if (componentsByType.TryGetValue(typeof(V), out var components))
                components.Remove(component);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Replaces one component with another and retrieves the replaces value when any was found
    /// </summary>
    /// <typeparam name="V">The type to to replace</typeparam>
    /// <param name="toReplace">Component to search for to be replaced</param>
    /// <param name="replacement">Component that replaces the <paramref name="toReplace"/></param>
    /// <param name="replaced">The item that equaled <paramref name="toReplace"/> and was replaced</param>
    /// <returns><see langword="true"/> if <paramref name="toReplace"/> was found, otherwise <see langword="false"/></returns>
    public virtual bool Replace<V>(V toReplace, V replacement, [MaybeNullWhen(false)] out V replaced) where V : T
    {

        if (!componentsByType.TryGetValue(typeof(V), out var components))
        {
            replaced = default;
            return false;
        }

        var index = components.IndexOf(toReplace);
        if (index < 0)
        {
            replaced = default;
            return false;
        }

        replaced = GenericCaster<T, V>.Cast(components[index]);
        components[index] = replacement;
        return true;
    }

    /// <summary>
    /// Replaces <paramref name="toReplace"/> with replacements or just adds <paramref name="replacement"/>, if <paramref name="toReplace"/> was not found
    /// </summary>
    /// <typeparam name="V">Type to add or replace</typeparam>
    /// <param name="toReplace">Component to search for to be replaced</param>
    /// <param name="replacement">Component that replaces the <paramref name="toReplace"/></param>
    /// <param name="replaced">The item that equaled <paramref name="toReplace"/> and was replaced</param>
    /// <returns><see langword="false"/> if only insert took place, otherwise <see langword="true"/> when <paramref name="toReplace"/> was found and replaced</returns>
    public virtual bool ReplaceOrAdd<V>(V? toReplace, V replacement, [MaybeNullWhen(false)] out V replaced) where V : T
    {
        replaced = default;

        if (componentsByType.TryGetValue(typeof(V), out var components))
        {
            if (toReplace is null)
            {
                components.Add(replacement);
            }
            else
            {
                var index = components.IndexOf(toReplace);
                replaced = GenericCaster<T, V>.Cast(components[index]);
                components[index] = replacement;
                return true;
            }
        }
        else
        {
            componentsByType[typeof(V)] = new List<T> { replacement };
        }
        return false;
    }

    /// <summary>
    /// Remove all components of type v and insert the replacement
    /// </summary>
    /// <typeparam name="V">Type that should be removed and inserted</typeparam>
    /// <param name="replacement">The only value after replacement</param>
    public virtual void ReplaceAllWith<V>(V replacement) where V : T
    {
        if (componentsByType.TryGetValue(typeof(V), out var components))
        {
            if (components.Count > 0)
            {
                components[0] = replacement;
                for (int i = components.Count - 1; i >= 1; i--)
                {
                    components.RemoveAt(i);
                }
            }
            else
                components.Add(replacement);
        }
        else
        {
            componentsByType[typeof(V)] = new List<T> { replacement };
        }
    }


    /// <summary>
    /// Serializes the component list to a binary writer..
    /// </summary>
    /// <param name="writer">The binary writer to serialize the component list to.</param>
    public virtual void Serialize(BinaryWriter writer)
    {
        writer.Write(flatComponents.Count);
        foreach (var component in flatComponents)
        {
            writer.Write(component.GetType().AssemblyQualifiedName!);
            component.Serialize(writer);
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

            if (!componentsByType.TryGetValue(type, out var _))
                componentsByType[type] = new();

            var component = GenericCaster<object, T>.Cast(TypeContainer.GetUnregistered(type));
            AddIfTypeNotExists(component);
            component.Deserialize(reader);
        }
    }
}
