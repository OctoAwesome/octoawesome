
using OctoAwesome.Caching;

using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    public class ComponentExtensionExtender : BaseExtensionExtender<ComponentContainer>
    {
        private readonly Dictionary<Type, List<Action<ComponentContainer>>> componentContainerExtender;

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
            if (!componentContainerExtender.TryGetValue(type, out list))
            {
                list = new List<Action<ComponentContainer>>();
                componentContainerExtender.Add(type, list);
            }
            list.Add(GenericCaster<Action<ComponentContainer>, Action<T>>.Cast(extenderDelegate));
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