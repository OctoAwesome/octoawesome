using System;
using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome
{

    public interface ITypeContainer : IDisposable
    {

        object? CreateObject(Type type);
        T? CreateObject<T>() where T : class;

        void Register(Type registrar, Type type, InstanceBehavior instanceBehavior);
        void Register<T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class;
        void Register<TRegistrar, T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class;
        void Register(Type registrar, Type type, object singleton);
        void Register<T>(T singleton) where T : class;
        void Register<TRegistrar, T>(object singleton) where T : class;

        bool TryGet(Type type, [MaybeNullWhen(false)] out object instance);
        bool TryGet<T>([MaybeNullWhen(false)] out T instance) where T : class;

        object Get(Type type);
        T Get<T>() where T : class;

        object GetUnregistered(Type type);
        T GetUnregistered<T>() where T : class;

        object? GetOrNull(Type type);
        T? GetOrNull<T>() where T : class;
    }
}