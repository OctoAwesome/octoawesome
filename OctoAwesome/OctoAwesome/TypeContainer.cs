using System;
using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome
{
    /// <summary>
    /// Static interface to a <see cref="StandaloneTypeContainer"/> singleton.
    /// </summary>
    public static class TypeContainer
    {
        private static readonly ITypeContainer instance;

        static TypeContainer()
        {
            instance = new StandaloneTypeContainer();
            instance.Register((StandaloneTypeContainer)instance);
            instance.Register<ITypeContainer, StandaloneTypeContainer>(instance);
        }

        /// <inheritdoc cref="ITypeContainer.CreateObject"/>
        public static object? CreateObject(Type type)
            => instance.CreateObject(type);

        /// <inheritdoc cref="ITypeContainer.CreateObject{T}"/>
        public static T? CreateObject<T>() where T : class
            => instance.CreateObject<T>();

        /// <inheritdoc cref="ITypeContainer.Register(Type, Type, InstanceBehavior)"/>
        public static void Register(Type registrar, Type type, InstanceBehavior instanceBehavior)
            => instance.Register(registrar, type, instanceBehavior);

        /// <inheritdoc cref="ITypeContainer.Register{T}(InstanceBehavior)"/>
        public static void Register<T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class

            => instance.Register<T>(instanceBehavior);
        /// <inheritdoc cref="ITypeContainer.Register{TRegistrar, T}(InstanceBehavior)"/>
        public static void Register<TRegistrar, T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class
            => instance.Register<TRegistrar, T>(instanceBehavior);

        /// <inheritdoc cref="ITypeContainer.Register(Type, Type, object)"/>
        public static void Register(Type registrar, Type type, object singleton)
             => instance.Register(registrar, type, singleton);

        /// <inheritdoc cref="ITypeContainer.Register{T}(T)"/>
        public static void Register<T>(T singleton) where T : class
             => instance.Register(singleton);

        /// <inheritdoc cref="ITypeContainer.Register{TRegistrar, T}(object)"/>
        public static void Register<TRegistrar, T>(object singleton) where T : class
             => instance.Register<TRegistrar, T>(singleton);

        /// <inheritdoc cref="ITypeContainer.TryGet"/>
        public static bool TryResolve(Type type, [MaybeNullWhen(false)] out object resolvedInstance)
             => instance.TryGet(type, out resolvedInstance);

        /// <inheritdoc cref="ITypeContainer.TryGet{T}"/>
        public static bool TryResolve<T>([MaybeNullWhen(false)] out T resolvedInstance) where T : class
            => instance.TryGet(out resolvedInstance);

        /// <inheritdoc cref="ITypeContainer.Get"/>
        public static object Get(Type type)
            => instance.Get(type);

        /// <inheritdoc cref="ITypeContainer.Get{T}"/>
        public static T Get<T>() where T : class
            => instance.Get<T>();

        /// <inheritdoc cref="ITypeContainer.GetOrNull"/>
        public static object? GetOrNull(Type type)
            => instance.GetOrNull(type);

        /// <inheritdoc cref="ITypeContainer.GetOrNull{T}"/>
        public static T? GetOrNull<T>() where T : class
            => instance.GetOrNull<T>();

        /// <inheritdoc cref="ITypeContainer.GetUnregistered"/>
        public static object GetUnregistered(Type type)
            => instance.GetUnregistered(type);

        /// <inheritdoc cref="ITypeContainer.GetUnregistered{T}"/>
        public static T GetUnregistered<T>() where T : class
            => instance.GetUnregistered<T>();

    }
}
