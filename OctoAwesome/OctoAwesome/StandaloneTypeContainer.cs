using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    /// Implementation for a standalone type container.
    /// </summary>
    public sealed class StandaloneTypeContainer : ITypeContainer
    {
        private readonly Dictionary<Type, TypeInformation> typeInformationRegister;
        private readonly Dictionary<Type, Type> typeRegister;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandaloneTypeContainer"/> class.
        /// </summary>
        public StandaloneTypeContainer()
        {
            typeInformationRegister = new Dictionary<Type, TypeInformation>();
            typeRegister = new Dictionary<Type, Type>();
        }


        /// <inheritdoc />
        public void Register(Type registrar, Type type, InstanceBehavior instanceBehavior)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehavior));

            typeRegister.Add(registrar, type);
        }

        /// <inheritdoc />
        public void Register<T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class
            => Register(typeof(T), typeof(T), instanceBehavior);

        /// <inheritdoc />
        public void Register<TRegistrar, T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class
            => Register(typeof(TRegistrar), typeof(T), instanceBehavior);

        /// <inheritdoc />
        public void Register(Type registrar, Type type, object singleton)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehavior.Singleton, singleton));

            typeRegister.Add(registrar, type);
        }

        /// <inheritdoc />
        public void Register<T>(T singleton) where T : class
            => Register(typeof(T), typeof(T), singleton);

        /// <inheritdoc />
        public void Register<TRegistrar, T>(object singleton) where T : class
            => Register(typeof(TRegistrar), typeof(T), singleton);

        /// <inheritdoc />
        public bool TryGet(Type type, [MaybeNullWhen(false)] out object instance)
        {
            instance = GetOrNull(type);
            return instance != null;
        }

        /// <inheritdoc />
        public bool TryGet<T>([MaybeNullWhen(false)] out T instance) where T : class
        {
            if (TryGet(typeof(T), out var obj))
            {
                instance = (T)obj;
                return true;
            }
            instance = null;
            return false;
        }

        /// <inheritdoc />
        public object Get(Type type)
            => GetOrNull(type) ?? throw new KeyNotFoundException($"Type {type} was not found in Container");

        /// <inheritdoc />
        public T Get<T>() where T : class
            => (T)Get(typeof(T));

        /// <inheritdoc />
        public object? GetOrNull(Type type)
        {
            if (typeRegister.TryGetValue(type, out var searchType))
            {
                if (typeInformationRegister.TryGetValue(searchType, out var typeInformation))
                    return typeInformation.Instance;
            }
            return null;
        }

        /// <inheritdoc />
        public T? GetOrNull<T>() where T : class
            => (T?)GetOrNull(typeof(T));

        /// <inheritdoc />
        public object GetUnregistered(Type type)
            => GetOrNull(type)
                ?? CreateObject(type)
                ?? throw new InvalidOperationException($"Can not create unregistered type of {type}");

        /// <inheritdoc />
        public T GetUnregistered<T>() where T : class
            => (T)GetUnregistered(typeof(T));

        /// <inheritdoc />
        public IEnumerable<Type> GetRegisteredTypes()
        {
            return typeInformationRegister.Keys;
        }

        /// <inheritdoc />
        public object? CreateObject(Type type)
        {
            var tmpList = new List<object?>();

            var constructors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                bool next = false;
                foreach (var parameter in constructor.GetParameters())
                {
                    if (TryGet(parameter.ParameterType, out var instance))
                    {
                        tmpList.Add(instance);
                    }
                    else if (parameter.IsOptional && parameter.HasDefaultValue)
                    {
                        tmpList.Add(parameter.DefaultValue);
                    }
                    else // Unknown parameter type without default value
                    {
                        tmpList.Clear();
                        next = true;
                        break;
                    }
                }

                if (next)
                    continue;

                return constructor.Invoke(tmpList.ToArray());
            }

            if (!constructors.Any())
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public T? CreateObject<T>() where T : class
            => (T?)CreateObject(typeof(T));

        /// <inheritdoc />
        public void Dispose()
        {
            typeRegister.Clear();
            typeInformationRegister.Values
                .Where(t => t.Behavior == InstanceBehavior.Singleton && t.Instance != this)
                .Select(t => t.Instance as IDisposable)
                .ToList()
                .ForEach(i => i?.Dispose());

            typeInformationRegister.Clear();
        }

        private class TypeInformation
        {
            public InstanceBehavior Behavior { get; set; }
            public object? Instance => CreateObject();

            private readonly StandaloneTypeContainer typeContainer;
            private readonly Type type;
            private object? singletonInstance;

            public TypeInformation(StandaloneTypeContainer container,
                Type type, InstanceBehavior instanceBehavior, object? instance = null)
            {
                this.type = type;
                Behavior = instanceBehavior;
                typeContainer = container;
                singletonInstance = instance;
            }

            private object? CreateObject()
            {
                if (Behavior == InstanceBehavior.Singleton && singletonInstance != null)
                    return singletonInstance;

                var obj = typeContainer.CreateObject(type);

                if (Behavior == InstanceBehavior.Singleton)
                    singletonInstance = obj;

                return obj;
            }
        }
    }
}
