using System;
using System.Collections;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Component based Entities.
    /// </summary>
    /// <typeparam name="T">Type of Component</typeparam>
    public abstract class ComponentList<T> : IEnumerable<T> where T : EntityComponent
    {
        private Dictionary<Type, T> components = new Dictionary<Type, T>();

        public T this[Type type]
        {
            get
            {
                T result;
                if (components.TryGetValue(type, out result))
                    return result;
                return null;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return components.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return components.Values.GetEnumerator();
        }

        /// <summary>
        /// Adds a new Component to the List.
        /// </summary>
        /// <param name="component">Component</param>
        public void AddComponent(T component)
        {
            Type type = component.GetType();
            components.Add(type, component);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ContainsComponent<T>()
        {
            return components.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Returns the Component of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>Component</returns>
        public V GetComponent<V>() where V : T
        {
            T result;
            if (components.TryGetValue(typeof(V), out result))
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
            return components.Remove(typeof(V));
        }
    }
}
