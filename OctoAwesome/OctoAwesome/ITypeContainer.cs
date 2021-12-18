using System;
using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for type containers.
    /// </summary>
    public interface ITypeContainer : IDisposable
    {
        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to create.</param>
        /// <returns>The created object instance; or <c>null</c> if no instance could be created.</returns>
        /// <seealso cref="CreateObject{T}"/>
        /// <remarks>The type does not need to be registered, but the constructor parameter types do.</remarks>
        object? CreateObject(Type type);

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <returns>The created object instance; or <c>null</c> if no instance could be created.</returns>
        /// <seealso cref="CreateObject"/>
        /// <remarks>The type does not need to be registered, but the constructor parameter types do.</remarks>
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