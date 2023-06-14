using OctoAwesome.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using OctoAwesome.Caching;
using NonSucking.Framework.Serialization;
using System.ComponentModel;

namespace OctoAwesome.Components;

/// <summary>
/// Base Class for all component based entities.
/// </summary>
/// <typeparam name="T">Type of the components contained in the list.</typeparam>
[NoosonCustom(DeserializeMethodName = nameof(DeserializeStatic), SerializeMethodName = nameof(Serialize))]
public class ComponentList<T> : IEnumerable<T> where T : IComponent, ISerializable
{

    /// <summary>
    /// Gets a component of a specific type; or <c>null</c> if no matching component is found.
    /// </summary>
    /// <param name="type">The type to get the component for</param>
    /// 
    public IList<T> this[Type type] => componentsByType.TryGetValue(type, out var result) ?
        result : Array.Empty<T>();

    [NoosonIgnore]
    private IReadOnlyCollection<Type> TypeKeys => componentsByType.Keys;
    internal IComponentContainer Parent
    {
        get => parent; set
        {
            parent = value;
            foreach (var item in flatComponents)
            {
                item.Parent = value;
            }
        }
    }

    private IComponentContainer parent;
    private readonly Action<T>? insertValidator;
    private readonly Action<T>? removeValidator;
    private readonly Action<T>? onInserter;
    private readonly Action<T>? onRemover;

    private readonly HashSet<T> flatComponents = new();
    private readonly Dictionary<Type, List<T>> componentsByType = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentList{T}"/> class.
    /// </summary>
    public ComponentList()
    {
        parent = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentList{T}"/> class.
    /// </summary>
    /// <param name="insertValidator">The validator for insertions.</param>
    /// <param name="removeValidator">The validator for removals.</param>
    /// <param name="onInserter">The method to call on insertion.</param>
    /// <param name="onRemover">The method to call on removal.</param>
    public ComponentList(Action<T>? insertValidator, Action<T>? removeValidator, Action<T>? onInserter, Action<T>? onRemover, IComponentContainer parent)
    {
        this.insertValidator = insertValidator;
        this.removeValidator = removeValidator;
        this.onInserter = onInserter;
        this.onRemover = onRemover;
        this.parent = parent;
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
        component.Parent = parent;
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

        component.Parent = parent;
        flatComponents.Add(component);
        insertValidator?.Invoke(component);
        onInserter?.Invoke(component);
    }


    /// <summary>
    /// Adds a component.
    /// </summary>
    /// <param name="component">The component to add if no component of same type is already present.</param>
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

        component.Parent = parent;
        flatComponents.Add(component);
        insertValidator?.Invoke(component);
        onInserter?.Invoke(component);
    }


    /// <summary>
    /// Checks whether the component of <typeparamref name="V"/> is present.
    /// </summary>
    /// <typeparam name="V">The type to search for.</typeparam>
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
    /// Tries to get the component of the given type.
    /// </summary>
    /// <typeparam name="V">The component type to search for.</typeparam>
    /// <returns><see langword="true"/> if the component was found; otherwise <see langword="false"/>.</returns>
    public bool TryGet<V>([MaybeNullWhen(false)] out V component) where V : IComponent
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
    /// Get the component of the given type.
    /// </summary>
    /// <typeparam name="V">The component type to search for.</typeparam>
    /// <returns>The component if found; otherwise <c>null</c>.</returns>
    public V? Get<V>()
    {
        if (componentsByType.TryGetValue(typeof(V), out var result) && result.Count > 0)
            return GenericCaster<T, V>.Cast(result[0]);

        return default;
    }

    /// <summary>
    /// Gets a list of components of the given type.
    /// </summary>
    /// <typeparam name="V">The component type to search for.</typeparam>
    /// <returns>A list of components if any was found; otherwise <c>null</c>.</returns>
    public List<V>? GetAll<V>()
    {
        if (componentsByType.TryGetValue(typeof(V), out var result))
            return GenericCaster<T, V>.CastList<List<T>, List<V>>(result);

        return default;
    }

    /// <summary>
    /// Removes the given component.
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
    /// Replace one component with another and retrieve the replaced value if one was found.
    /// </summary>
    /// <typeparam name="V">The type to search for and replace.</typeparam>
    /// <param name="toReplace">The component to search for to be replaced.</param>
    /// <param name="replacement">The component that replaces <paramref name="toReplace"/>.</param>
    /// <param name="replaced">The item that was equal to <paramref name="toReplace"/> and was replaced</param>
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

        replaced = GenericCaster<T, V>.Cast(components[index])!;
        components[index] = replacement;
        return true;
    }

    /// <summary>
    /// Replace <paramref name="toReplace"/> with <paramref name="replacement"/> or just add <paramref name="replacement"/>, if <paramref name="toReplace"/> was not found.
    /// </summary>
    /// <typeparam name="V">The component type to add or replace</typeparam>
    /// <param name="toReplace">The component to search for to be replaced.</param>
    /// <param name="replacement">The component that replaces <paramref name="toReplace"/>.</param>
    /// <param name="replaced">The item that was equal to <paramref name="toReplace"/> and was replaced.</param>
    /// <returns><see langword="false"/> if only insert took place, otherwise <see langword="true"/> when <paramref name="toReplace"/> was found and replaced</returns>
    public virtual bool ReplaceOrAdd<V>(V? toReplace, V replacement, [MaybeNullWhen(false)] out V replaced) where V : T
    {
        replaced = default;

        replacement.Parent = parent;
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
    /// Remove all components of a given type and insert a single component in place of the removed ones.
    /// </summary>
    /// <typeparam name="V">Type of the components to be replaced and replaced with.</typeparam>
    /// <param name="replacement">The value to replace with.</param>
    public virtual void ReplaceAllWith<V>(V replacement) where V : T
    {
        replacement.Parent = parent;
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
    /// Deserializes the component list from a binary reader.
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

            var component = (T)TypeContainer.GetUnregistered(type);
            component.Deserialize(reader);
            AddIfTypeNotExists(component);
        }
    }


    /// <summary>
    /// Serializes the component list to a binary writer.
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
    /// Deserializes the component list from a binary reader.
    /// </summary>
    /// <param name="reader">The binary reader to deserialize the component list from.</param>
    public static ComponentList<T> DeserializeStatic(BinaryReader reader)
    {
        var ret = new ComponentList<T>();
        ret.Deserialize(reader);
        return ret;
    }
}
