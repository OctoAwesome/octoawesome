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
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

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
                item.Value.Parent = value;
            }
        }
    }

    private IComponentContainer? parent;
    private readonly IResourceManager resourceManager;
    private readonly Action<T>? insertValidator;
    private readonly Action<T>? removeValidator;
    private readonly Action<T>? onInserter;
    private readonly Action<T>? onRemover;

    private readonly Dictionary<int, T> flatComponents = new();
    private readonly Dictionary<Type, List<T>> componentsByType = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentList{T}"/> class.
    /// </summary>
    public ComponentList()
    {
        parent = default;
        resourceManager = TypeContainer.Get<IResourceManager>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentList{T}"/> class.
    /// </summary>
    /// <param name="insertValidator">The validator for insertions.</param>
    /// <param name="removeValidator">The validator for removals.</param>
    /// <param name="onInserter">The method to call on insertion.</param>
    /// <param name="onRemover">The method to call on removal.</param>
    public ComponentList(Action<T>? insertValidator, Action<T>? removeValidator, Action<T>? onInserter, Action<T>? onRemover, IComponentContainer parent) : this()
    {
        this.insertValidator = insertValidator;
        this.removeValidator = removeValidator;
        this.onInserter = onInserter;
        this.onRemover = onRemover;
        this.parent = parent;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => flatComponents.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => flatComponents.Values.GetEnumerator();

    /// <summary>
    /// Adds a component if the type is not already present.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <typeparam name="V">The type of the component to add.</typeparam>
    public void AddIfTypeNotExists<V>(V component) where V : T
    {
        Type type = component.GetType();
        AssignId(component);

        if (!componentsByType.TryGetValue(type, out var existing))
        {
            existing = new();
            componentsByType[type] = existing;
        }
        else if (existing.Count > 0)
        {
            return;
        }

        flatComponents[component.Id] = component;

        existing.Add(component);
        if (parent is not null)
            component.Parent = parent;
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
        AssignId(component);

        ref var comp = ref CollectionsMarshal.GetValueRefOrAddDefault(flatComponents, component.Id, out var exists);
        if (exists)
            return;


        if (componentsByType.TryGetValue(type, out var existing))
        {
            existing.Add(component);
        }
        else
        {
            componentsByType[type] = new() { component };
        }

        if (parent is not null)
            component.Parent = parent;
        comp = component;
        insertValidator?.Invoke(component);
        onInserter?.Invoke(component);
    }


    /// <summary>
    /// Adds a component, if the id of the component is not already contained in this component list.
    /// </summary>
    /// <param name="component">The component to add if no component of same id is already present.</param>
    /// <typeparam name="V">The type of the component to add.</typeparam>
    public void AddIfNotExists<V>(V component) where V : T
    {
        Type type = component.GetType();
        AssignId(component);
        ref var comp = ref CollectionsMarshal.GetValueRefOrAddDefault(flatComponents, component.Id, out var exists);
        if (exists)
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

        if (parent is not null)
            component.Parent = parent;
        comp = component;
        insertValidator?.Invoke(component);
        onInserter?.Invoke(component);
    }

    private void AssignId(IComponent component)
    {
        if (component.Id == -1)
            component.Id = resourceManager.IdManager.GetNextId();
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
    /// Checks whether the component with <paramref name="id"/> is present.
    /// </summary>
    /// <returns>
    /// <list type="bullet">
    ///     <item><see langword="true"/> if the component was found</item>
    ///     <item><see langword="false"/> if the component was not found</item>
    /// </list>
    /// </returns>
    public bool Contains(int id)
        => flatComponents.ContainsKey(id);


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
    /// Get the component of the given type and id.
    /// </summary>
    /// <param name="id">The unique identifier for the component</param>
    /// <typeparam name="V">The component type to search for.</typeparam>
    /// <returns>The component if found; otherwise <c>null</c>.</returns>
    public V? Get<V>(int id)
    {
        if (flatComponents.TryGetValue(id, out var result))
        {
            return GenericCaster<T, V>.Cast(result);
        }

        return default;
    }

    /// <summary>
    /// Get the component of the given type and id.
    /// </summary>
    /// <param name="id">The unique identifier for the component</param>
    /// <returns>The component if found; otherwise <c>null</c>.</returns>
    public IComponent? Get(int id) => flatComponents.TryGetValue(id, out T? result) ? result : (IComponent?)default;


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
        if (!flatComponents.ContainsKey(component.Id))
            return false;

        removeValidator?.Invoke(component);
        if (flatComponents.Remove(component.Id))
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
        AssignId(replacement);

        ref var comp = ref CollectionsMarshal.GetValueRefOrNullRef(flatComponents, toReplace.Id);
        if (Unsafe.IsNullRef(ref comp))
        {
            replaced = default;
            return false;
        }


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
        comp = replacement;
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
        AssignId(replacement);

        replacement.Parent = parent;

        if (toReplace is not null)
            flatComponents.Remove(toReplace.Id);
        flatComponents[replacement.Id] = replacement;


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
        AssignId(replacement);
        replacement.Parent = parent;
        if (componentsByType.TryGetValue(typeof(V), out var components))
        {
            if (components.Count > 0)
            {
                for (int i = components.Count - 1; i >= 0; i--)
                {
                    var comp = components[i];
                    flatComponents.Remove(comp.Id);
                    components.RemoveAt(i);
                }
            }
            components.Add(replacement);
        }
        else
        {
            componentsByType[typeof(V)] = new List<T> { replacement };
        }
        flatComponents[replacement.Id] = replacement;
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
            var serId = reader.ReadUInt64();

            var type = SerializationIdTypeProvider.Get(serId);

            Debug.Assert(type != null, $"{nameof(type)} is null for serid: {serId}");

            ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(componentsByType, type, out var exists);
            if (!exists || list is null)
                list = new();

            var component = (T)TypeContainer.GetUnregistered(type);
            component.Deserialize(reader);
            list.Add(component);
            flatComponents[component.Id] = component;
        }
    }


    /// <summary>
    /// Serializes the component list to a binary writer.
    /// </summary>
    /// <param name="writer">The binary writer to serialize the component list to.</param>
    public virtual void Serialize(BinaryWriter writer)
    {
        writer.Write(flatComponents.Count);
        foreach (var keyValuePair in flatComponents)
        {
            var comp = keyValuePair.Value;
            writer.Write(comp.GetType().Name);
            writer.Write(comp.GetType().SerializationId());
            comp.Serialize(writer);
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