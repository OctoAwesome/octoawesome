using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Component based Entities.
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    public class ComponentList<T> : IEnumerable<T> where T : Component
    {
        /// <summary>
        /// On add delegate.
        /// </summary>
        private Action<T> onInserter;
        /// <summary>
        /// On remove delegate.
        /// </summary>
        private Action<T> onRemover;
        /// <summary>
        /// Backend storage for Components.
        /// </summary>
        private readonly Dictionary<Type, T> components = new Dictionary<Type, T>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
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
        /// Default Constructor.
        /// </summary>
        public ComponentList()
        {
        }
        /// <summary>
        /// Componentstorage Constructor with onAdd and onRemove delegate.
        /// </summary>
        /// <param name="onAdd">Invoked before the Component is Added.</param>
        /// <param name="onRemove">Invoked before the Component is Removed.</param>
        public ComponentList(Action<T> onAdd, Action<T> onRemove)
        {
            onInserter = onAdd;
            onRemover = onRemove;
        }
        /// <summary>
        /// Return Enumerator of Components.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => components.Values.GetEnumerator();
        /// <summary>
        /// Return Enumerator of Components.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => components.Values.GetEnumerator();
        /// <summary>
        /// Adds a new Component to the List.
        /// </summary>
        /// <param name="component">Component.</param>
        public void AddComponent<V>(V component) where V : T
        {
            AddComponent(component, false);
        }
        /// <summary>
        /// Adds a new Component to the List.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="component"></param>
        /// <param name="replace"></param>
        public void AddComponent<V>(V component, bool replace) where V : T
        {
            Type type = component.GetType();
            if (components.ContainsKey(type))
            {
                if (replace) components.Remove(type);
                else return;
            }
            onInserter?.Invoke(component);
            components.Add(type, component);
        }
        /// <summary>
        /// Check if the type is included.
        /// </summary>
        /// <returns></returns>
        public bool ContainsComponent<V>() where V : T
        {
            return components.ContainsKey(typeof(V));
        }
        /// <summary>
        /// Check if the type is included.
        /// </summary>
        /// <param name="type">Type of the Component.</param>
        /// <returns></returns>
        public bool ContainsComponent(Type type)
        {
            return components.ContainsKey(type);
        }
        /// <summary>
        /// Try get a Component.
        /// </summary>
        /// <typeparam name="V">Casting the Component to type of V.</typeparam>
        /// <param name="type">Type of the Component.</param>
        /// <param name="component">out Component.</param>
        /// <returns></returns>
        public bool TryGetComponent<V>(Type type, out V component) where V : T
        {
            bool result = components.TryGetValue(type, out T comp);
            component = comp as V;
            return component != null;
        }
        /// <summary>
        /// Try get a Component.
        /// </summary>
        /// <typeparam name="V">Casting the Component to type of V.</typeparam>
        /// <param name="component">out Component.</param>
        /// <returns></returns>
        public bool TryGetComponent<V>(out V component) where V : T
        {
            bool result = components.TryGetValue(typeof(V), out T comp);
            component = comp as V;
            return component != null;
        }
        /// <summary>
        /// Get a Component with the given Type.
        /// </summary>
        /// <typeparam name="V">Type of the Component.</typeparam>
        /// <returns></returns>
        public V GetComponent<V>() where V : T
        {
            if (components.TryGetValue(typeof(V), out T result))
                return result as V;
            return null;
        }
        /// <summary>
        /// Get a Component with the given Type.
        /// </summary>
        /// <typeparam name="V">Type of the Component.</typeparam>
        /// <param name="type">Type of the Component.</param>
        /// <returns></returns>
        public V GetComponent<V>(Type type) where V : T
        {
            if (components.TryGetValue(type, out T result))
                return result as V;
            return null;
        }
        /// <summary> 
        /// Removes the Component of the given Type.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public bool RemoveComponent<V>() where V : T
        {
            if (components.TryGetValue(typeof(V), out T component))
            {
                onRemover?.Invoke(component);
                return components.Remove(typeof(V));
            }
            return false;
        }
        /// <summary>
        /// Removes the Component of the given Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool RemoveComponent(Type type)
        {
            if (components.TryGetValue(type, out T component))
            {
                onRemover?.Invoke(component);
                return components.Remove(type);
            }
            return false;
        }
        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            writer.Write(components.Count);
            //TODO: ähh was?
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

                    if (!components.TryGetValue(type, out T component))
                    {
                        component = (T) Activator.CreateInstance(type);
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
