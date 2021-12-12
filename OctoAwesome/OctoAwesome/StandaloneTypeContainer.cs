using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OctoAwesome
{

    public sealed class StandaloneTypeContainer : ITypeContainer
    {
        private readonly Dictionary<Type, TypeInformation> typeInformationRegister;
        private readonly Dictionary<Type, Type> typeRegister;
        public StandaloneTypeContainer()
        {
            typeInformationRegister = new Dictionary<Type, TypeInformation>();
            typeRegister = new Dictionary<Type, Type>();
        }

        public void Register(Type registrar, Type type, InstanceBehavior instanceBehavior)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehavior));

            typeRegister.Add(registrar, type);
        }
        public void Register<T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class
            => Register(typeof(T), typeof(T), instanceBehavior);
        public void Register<TRegistrar, T>(InstanceBehavior instanceBehavior = InstanceBehavior.Instance) where T : class
            => Register(typeof(TRegistrar), typeof(T), instanceBehavior);
        public void Register(Type registrar, Type type, object singleton)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehavior.Singleton, singleton));

            typeRegister.Add(registrar, type);
        }
        public void Register<T>(T singleton) where T : class
            => Register(typeof(T), typeof(T), singleton);
        public void Register<TRegistrar, T>(object singleton) where T : class
            => Register(typeof(TRegistrar), typeof(T), singleton);
        public bool TryGet(Type type, [MaybeNullWhen(false)] out object instance)
        {
            instance = GetOrNull(type);
            return instance != null;
        }
        public bool TryGet<T>([MaybeNullWhen(false)] out T instance) where T : class
        {
            var result = TryGet(typeof(T), out var obj);
            if (result)
                instance = (T)obj!;
            else
                instance = null;
            return result;
        }
        public object Get(Type type)
            => GetOrNull(type) ?? throw new KeyNotFoundException($"Type {type} was not found in Container");
        public T Get<T>() where T : class
            => (T)Get(typeof(T));
        public object? GetOrNull(Type type)
        {
            if (typeRegister.TryGetValue(type, out var searchType))
            {
                if (typeInformationRegister.TryGetValue(searchType, out var typeInformation))
                    return typeInformation.Instance;
            }
            return null;
        }
        public T? GetOrNull<T>() where T : class
            => (T?)GetOrNull(typeof(T));
        public object GetUnregistered(Type type)
            => GetOrNull(type)
                ?? CreateObject(type)
                ?? throw new InvalidOperationException($"Can not create unregistered type of {type}");
        public T GetUnregistered<T>() where T : class
            => (T)GetUnregistered(typeof(T));
        public object? CreateObject(Type type)
        {
            var tmpList = new List<object>();

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
                    else if (!parameter.IsOptional)
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
        public T? CreateObject<T>() where T : class
            => (T?)CreateObject(typeof(T));
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
