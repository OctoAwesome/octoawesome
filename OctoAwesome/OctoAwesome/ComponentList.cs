using OctoAwesome.Serialization;
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
    public class ComponentList<T> : IEnumerable<T> where T : Component, ISerializable
    {
        private readonly Action<T> insertValidator;
        private readonly Action<T> removeValidator;
        private readonly Action<T> onInserter;
        private readonly Action<T> onRemover;

        private readonly Dictionary<Type, T> components = new Dictionary<Type, T>();

        public T this[Type type]
        {
            get
            {
                if (components.TryGetValue(type, out T result))
                    return result;

                return null;
            }
        }

        public ComponentList()
        {
        }

        public ComponentList(Action<T> insertValidator, Action<T> removeValidator, Action<T> onInserter, Action<T> onRemover)
        {
            this.insertValidator = insertValidator;
            this.removeValidator = removeValidator;
            this.onInserter = onInserter;
            this.onRemover = onRemover;
        }

        public IEnumerator<T> GetEnumerator()
            => components.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => components.Values.GetEnumerator();

        /// <summary>
        /// Adds a new Component to the List.
        /// </summary>
        /// <param name="component">Component</param>
        public void AddComponent<V>(V component) where V : T 
            => AddComponent(component, false);


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
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public bool ContainsComponent<V>() 
            => components.ContainsKey(typeof(V));

        /// <summary>
        /// Returns the Component of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>Component</returns>
        public V GetComponent<V>() where V : T
        {
            if (components.TryGetValue(typeof(V), out T result))
                return (V)result;

            return null;
        }

        /// <summary>
        /// Removes the Component of the given Type.
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns></returns>
        public bool RemoveComponent<V>() where V : T
        {
            if (!components.TryGetValue(typeof(V), out T component))
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
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(components.Count);
            foreach (var componente in components)
            {
                writer.Write(componente.Key.AssemblyQualifiedName);
                componente.Value.Serialize(writer);

            }
        }

        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        public virtual void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();

                var type = Type.GetType(name);

                T component;

                if (!components.TryGetValue(type, out component))
                {
                    component = (T)Activator.CreateInstance(type);
                    //components.Add(type, component);
                    AddComponent(component);
                }

                component.Deserialize(reader);
            }
        }
    }
}
