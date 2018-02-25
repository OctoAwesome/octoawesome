using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Component based Entities.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of Component</typeparam>
    public class ComponentList<T> : ISerializable, IEnumerable<T> where T : Component
    {
        private Action<T> onAdder;
        private Action<T> onRemover;
        private readonly Dictionary<Type, T> components = new Dictionary<Type, T>();
        /// <summary>
        /// Return an <see cref="Component"/> of the <see cref="ComponentList{T}"/>.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the <see cref="T"/></param>
        /// <returns></returns>
        public T this[Type type]
        {
            get
            {
                if (components.TryGetValue(type, out T result))
                    return result;
                return null;
            }
        }
        /// <summary>
        /// Default Constructor of <see cref="ComponentList{T}"/>.
        /// </summary>
        public ComponentList()
        {
        }
        /// <summary>
        /// Default Constructor of <see cref="ComponentList{T}"/>.
        /// </summary>
        /// <param name="onAdd">On <see cref="{T}"/> add delegate.</param>
        /// <param name="onRemove">On <see cref="{T}"/> remove delegate.</param>
        public ComponentList(Action<T> onAdd, Action<T> onRemove)
        {
            onAdder = onAdd;
            onRemover = onRemove;
        }
        /// <summary>
        /// <see cref="IEnumerator{T}"/> of the List.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => components.Values.GetEnumerator();
        /// <summary>
        /// <see cref="IEnumerator{T}"/> of the List.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => components.Values.GetEnumerator();
        /// <summary>
        /// Adds a new <see cref="Component"/> to the List.
        /// </summary>
        /// <param name="component">Component</param>
        public void AddComponent<V>(V component) where V : T
        {
            AddComponent(component, false);
        }
        /// <summary>
        /// Adds a new <see cref="Component"/> to the List and replace Items with the same Key.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="component">Component</param>
        /// <param name="replace">Replace i already inclueded.</param>
        public void AddComponent<V>(V component, bool replace) where V : T
        {
            AddComponent<V>(typeof(V), component, replace);
        }
        /// <summary>
        /// Adds a new <see cref="Component"/> to the List and replace Items with the same Key.
        /// </summary>
        /// <typeparam name="V">Type of Component.</typeparam>
        /// <param name="type">Type.</param>
        /// <param name="component">Component.</param>
        /// <param name="replace">Replace i already inclueded.</param>
        public void AddComponent<V>(Type type, V component, bool replace) where V : T
        {
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
            onAdder?.Invoke(component);
            components.Add(type, component);
        }
        /// <summary>
        /// Checks if the <see cref="Component"/> is included.
        /// </summary>
        /// <typeparam name="V"><see cref="Type"/> of the <see cref="Component"/>.</typeparam>
        /// <returns></returns>
        public bool ContainsComponent<V>() where V : T
        {
            return components.ContainsKey(typeof(V));
        }
        /// <summary>
        /// Checks if the <see cref="Component"/> is included.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the <see cref="Component"/>.</param>
        /// <returns></returns>
        public bool ContainsComponent(Type type)
        {
            return components.ContainsKey(type);
        }
        /// <summary>
        /// Returns the <see cref="Component"/> of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>Component</returns>
        public V GetComponent<V>() where V : T
        {
            return GetComponent<V>(typeof(V));
        }
        /// <summary>
        /// Returns the <see cref="Component"/> of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <param name="type">Type.</param>
        /// <returns></returns>
        public V GetComponent<V>(Type type) where V : T
        {
            if (components.TryGetValue(type, out T result))
                return (V) result;
            return null;
        }
        /// <summary>
        /// Try to get a <see cref="Component"/>.
        /// </summary>
        /// <typeparam name="V"><see cref="Type"/> of <see cref="Component"/></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetComponent<V>(out V component) where V : T
        {
            return TryGetComponent<V>(typeof(V), out component);
        }
        /// <summary>
        /// Try to get a <see cref="Component"/>.
        /// </summary>
        /// <typeparam name="V"><see cref="Type"/> of <see cref="Component"/></typeparam>
        /// <param name="type">Type</param>
        /// <param name="component">Component</param>
        /// <returns></returns>
        public bool TryGetComponent<V>(Type type, out V component) where V : T
        {
            T comp;
            components.TryGetValue(type, out comp);
            component = comp as V;
            return component != null;
        }
        /// <summary>
        /// Removes the <see cref="Component"/> of the given Type.
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns></returns>
        public bool RemoveComponent<V>() where V : T
        {
            T component;
            if (!components.TryGetValue(typeof(V), out component))
                return false;
            else onRemover?.Invoke(component);
            return components.Remove(typeof(V));
        }
        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            writer.Write(components.Count);
            foreach (var componente in components)
            {
                using (MemoryStream memorystream = new MemoryStream())
                {
                    writer.Write(componente.Key.AssemblyQualifiedName);

                    using (BinaryWriter componentbinarystream = new BinaryWriter(memorystream))
                    {
                        try
                        {
                            componente.Value.Serialize(componentbinarystream, definitionManager);
                            writer.Write((int)memorystream.Length);
                            memorystream.WriteTo(writer.BaseStream);

                        }
                        catch (Exception)
                        {
                            writer.Write(0);
                            //throw; //TODO #CleanUp throw?
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var length = reader.ReadInt32();

                var startPosition = reader.BaseStream.Position;

                try
                {
                    var type = Type.GetType(name);

                    if (type == null)
                        continue;

                    T component;

                    if (!components.TryGetValue(type, out component))
                    {
                        component = (T)Activator.CreateInstance(type);
                        components.Add(type, component);
                    }

                    byte[] buffer = new byte[length];
                    reader.Read(buffer, 0, length);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (BinaryReader componentBinaryStream = new BinaryReader(memoryStream))
                        {
                            component.Deserialize(componentBinaryStream, definitionManager);
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    reader.BaseStream.Position = startPosition + length;
                }
            }
        }
    }
}
