
using OctoAwesome.Caching;

using System;
using System.Collections.Generic;

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
            List<Action<ComponentContainer>> list;
            if (!typeof(ComponentContainer).IsAssignableFrom(typeof(T)))
                return;
            if (!componentContainerExtender.TryGetValue(type, out list))
            {
                list = new List<Action<ComponentContainer>>();
                componentContainerExtender.Add(type, list);
            }
            if (extenderDelegate is Action<ComponentContainer> ccAction)
                list.Add(ccAction);
            else
                list.Add((ComponentContainer cc) => { extenderDelegate(GenericCaster<ComponentContainer, T>.Cast(cc)); });
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
                t = t!.BaseType;
                stack.Add(t);
            }
            while (t != typeof(ComponentContainer));
            stack.Reverse();

            foreach (var type in stack)
            {
                List<Action<ComponentContainer>> list;
                if (!componentContainerExtender.TryGetValue(type, out list))
                    continue;

                foreach (var item in list)
                    item(instance);
            }
        }
    }
}