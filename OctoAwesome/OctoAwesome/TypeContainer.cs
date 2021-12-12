using System;
using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome
{

    public static class TypeContainer
    {
        private static readonly ITypeContainer instance;

        static TypeContainer()
        {
            instance = new StandaloneTypeContainer();
            instance.Register((StandaloneTypeContainer)instance);
            instance.Register<ITypeContainer, StandaloneTypeContainer>(instance);
        }
        public static object? CreateObject(Type type)
            => instance.CreateObject(type);
        public static T? CreateObject<T>() where T : class
            => instance.CreateObject<T>();
        public static void Register(Type registrar, Type type, InstanceBehavior instanceBehavior)
            => instance.Register(registrar, type, instanceBehavior);
        public static void Register<T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class

            => instance.Register<T>(instanceBehavior);

        public static void Register<TRegistrar, T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class
            => instance.Register<TRegistrar, T>(instanceBehavior);
        public static void Register(Type registrar, Type type, object singleton)
             => instance.Register(registrar, type, singleton);
        

        public static void Register<T>(T singleton) where T : class
             => instance.Register(singleton);
        public static void Register<TRegistrar, T>(object singleton) where T : class
             => instance.Register<TRegistrar, T>(singleton);
        public static bool TryResolve(Type type, [MaybeNullWhen(false)] out object resolvedInstance)
             => instance.TryGet(type, out resolvedInstance);
        public static bool TryResolve<T>([MaybeNullWhen(false)] out T resolvedInstance) where T : class
            => instance.TryGet(out resolvedInstance);
        public static object Get(Type type)
            => instance.Get(type);
        

        public static T Get<T>() where T : class
            => instance.Get<T>();
        public static object? GetOrNull(Type type)
            => instance.GetOrNull(type);
        public static T? GetOrNull<T>() where T : class
            => instance.GetOrNull<T>();
        public static object GetUnregistered(Type type)
            => instance.GetUnregistered(type);
        public static T GetUnregistered<T>() where T : class
            => instance.GetUnregistered<T>();

    }
}
