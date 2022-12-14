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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrar"></param>
        /// <param name="type"></param>
        /// <param name="instanceBehavior"></param>
        void Register(Type registrar, Type type, InstanceBehavior instanceBehavior);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceBehavior"></param>
        /// <typeparam name="T"></typeparam>
        void Register<T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceBehavior"></param>
        /// <typeparam name="TRegistrar"></typeparam>
        /// <typeparam name="T"></typeparam>
        void Register<TRegistrar, T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrar"></param>
        /// <param name="type"></param>
        /// <param name="singleton"></param>
        void Register(Type registrar, Type type, object singleton);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleton"></param>
        /// <typeparam name="T"></typeparam>
        void Register<T>(T singleton) where T : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleton"></param>
        /// <typeparam name="TRegistrar"></typeparam>
        /// <typeparam name="T"></typeparam>
        void Register<TRegistrar, T>(object singleton) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool TryGet(Type type, [MaybeNullWhen(false)] out object instance);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool TryGet<T>([MaybeNullWhen(false)] out T instance) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Get(Type type);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object GetUnregistered(Type type);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetUnregistered<T>() where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object? GetOrNull(Type type);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T? GetOrNull<T>() where T : class;

        T? Remove<T>() where T : class;
    }
}