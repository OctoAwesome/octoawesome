using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehaviour));

            typeRegister.Add(registrar, type);
        }
        public void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => Register(typeof(T), typeof(T), instanceBehaviour);
        public void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => Register(typeof(TRegistrar), typeof(T), instanceBehaviour);
        public void Register(Type registrar, Type type, object singelton)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehaviour.Singleton, singelton));

            typeRegister.Add(registrar, type);
        }
        public void Register<T>(T singelton) where T : class
            => Register(typeof(T), typeof(T), singelton);
        public void Register<TRegistrar, T>(object singelton) where T : class
            => Register(typeof(TRegistrar), typeof(T), singelton);

        public bool TryResolve(Type type, out object instance)
        {
            instance = GetOrNull(type);
            return instance != null;
        }
        public bool TryResolve<T>(out T instance) where T : class
        {
            var result = TryResolve(typeof(T), out var obj);
            instance = (T)obj;
            return result;
        }

        public object Get(Type type)
            => GetOrNull(type) ?? throw new KeyNotFoundException($"Type {type} was not found in Container");

        public T Get<T>() where T : class
            => (T)Get(typeof(T));

        public object GetOrNull(Type type)
        {
            if (typeRegister.TryGetValue(type, out var searchType))
            {
                if (typeInformationRegister.TryGetValue(searchType, out var typeInformation))
                    return typeInformation.Instance;
            }
            return null;
        }
        public T GetOrNull<T>() where T : class
            => (T)GetOrNull(typeof(T));

        public object GetUnregistered(Type type)
            => GetOrNull(type)
                ?? CreateObject(type)
                ?? throw new InvalidOperationException($"Can not create unregistered type of {type}");

        public T GetUnregistered<T>() where T : class
            => (T)GetUnregistered(typeof(T));

        public object CreateObject(Type type)
        {
            var tmpList = new List<object>();

            var constructors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                bool next = false;
                foreach (var parameter in constructor.GetParameters())
                {
                    if (TryResolve(parameter.ParameterType, out object instance))
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

            if (constructors.Count() < 1)
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
        public T CreateObject<T>() where T : class
            => (T)CreateObject(typeof(T));

        public void Dispose()
        {
            typeRegister.Clear();
            typeInformationRegister.Values
                .Where(t => t.Behaviour == InstanceBehaviour.Singleton && t.Instance != this)
                .Select(t => t.Instance as IDisposable)
                .ToList()
                .ForEach(i => i?.Dispose());

            typeInformationRegister.Clear();
        }

        private class TypeInformation
        {
            public InstanceBehaviour Behaviour { get; set; }
            public object Instance => CreateObject();

            private readonly StandaloneTypeContainer typeContainer;
            private readonly Type type;
            private object singeltonInstance;

            public TypeInformation(StandaloneTypeContainer container,
                Type type, InstanceBehaviour instanceBehaviour, object instance = null)
            {
                this.type = type;
                Behaviour = instanceBehaviour;
                typeContainer = container;
                singeltonInstance = instance;
            }

            private object CreateObject()
            {
                if (Behaviour == InstanceBehaviour.Singleton && singeltonInstance != null)
                    return singeltonInstance;

                var obj = typeContainer.CreateObject(type);

                if (Behaviour == InstanceBehaviour.Singleton)
                    singeltonInstance = obj;

                return obj;
            }
        }
    }
}
