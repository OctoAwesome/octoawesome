
using OctoAwesome.Caching;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OctoAwesome
{
    /// <summary>
    /// Extender for component containers
    /// </summary>
    public class ComponentExtensionExtender : BaseExtensionExtender<ComponentContainer>
    {
        private readonly Dictionary<Type, List<Action<ComponentContainer>>> componentContainerExtender;        

        /// <summary>
        /// Initializes a new instance of the<see cref="ComponentExtensionExtender" /> class
        /// </summary>
        public ComponentExtensionExtender()
        {
            componentContainerExtender = new Dictionary<Type, List<Action<ComponentContainer>>>();

        }

        /// <summary>
        /// Adds a new Extender for the given Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="extenderDelegate">Extender Delegate</param>
        public override void RegisterExtender<T>(Action<T> extenderDelegate)
        {
            Type type = typeof(T);
            if (!typeof(ComponentContainer).IsAssignableFrom(typeof(T)))
                return;
            if (!componentContainerExtender.TryGetValue(type, out var list))
            {
                list = new List<Action<ComponentContainer>>();
                componentContainerExtender.Add(type, list);
            }
            if (extenderDelegate is Action<ComponentContainer> ccAction)
                list.Add(ccAction);
            else
                list.Add((ComponentContainer cc) => { extenderDelegate(GenericCaster<ComponentContainer, T>.Cast(cc)!); });
        }

        /// <summary>
        /// Adds the Default Extender for the given Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        public void ExtendDefault<T>() where T : ComponentContainer
            => RegisterExtender<T>((e) => e.RegisterDefault());

        /// <summary>
        /// Extend a Entity
        /// </summary>
        /// <param name="instance">Entity</param>
        public override void Execute<T>(T instance)
        {
            List<Type> stack = new List<Type>();
            Type t = instance.GetType();
            stack.Add(t);
            do
            {
                var baseType = t.BaseType;
                Debug.Assert(baseType != null, nameof(baseType) + $" != null. T needs to inherit from {nameof(ComponentContainer)}");
                t = baseType;
                stack.Add(t);
            }
            while (t != typeof(ComponentContainer));
            stack.Reverse();

            foreach (var type in stack)
            {
                if (!componentContainerExtender.TryGetValue(type, out var list))
                    continue;

                foreach (var item in list)
                    item(instance);
            }
        }
    }
}