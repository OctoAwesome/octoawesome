using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class StandaloneTypeContainer
    {

        private readonly Dictionary<Type, TypeInformation> typeInformationRegister;
        private readonly Dictionary<Type, Type> typeRegister;

        public StandaloneTypeContainer()
        {
            typeInformationRegister = new Dictionary<Type, TypeInformation>();
            typeRegister = new Dictionary<Type, Type>();
        }


        public void Register(Type type, InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance)
        {
            typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehaviour));
        }
        public void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, instanceBehaviour));

            typeRegister.Add(registrar, type);
        }

        public bool TryResolve(Type type, out object instance)
        {
            if (typeInformationRegister.TryGetValue(type, out TypeInformation typeInformation))
            {
                instance = typeInformation.Instance;
                return true;
            }

            if (typeRegister.TryGetValue(type, out var searchType))
            {
                if (typeInformationRegister.TryGetValue(searchType, out typeInformation))
                {
                    instance = typeInformation.Instance;
                    return true;
                }
            }

            instance = Activator.CreateInstance(type);
            return instance == null;
        }

        public object CreateObject(Type type)
        {
            var tmpList = new List<object>();
            foreach (var constructor in type.GetConstructors().OrderByDescending(c => c.GetParameters().Length))
            {
                bool next = false;                
                foreach (var parameter in constructor.GetParameters())
                {
                    if (TryResolve(parameter.ParameterType, out object instance))
                    {
                        tmpList.Add(instance);
                    }
                    else
                    {
                        tmpList.Clear();
                        next = true;
                        break;
                    }
                }

                if (next)
                    continue;

                return constructor.Invoke(type, tmpList.ToArray());
            }

            return null;
        }

        public enum InstanceBehaviour
        {
            Instance,
            Singleton
        }

        private class TypeInformation
        {
            public InstanceBehaviour Behaviour { get; set; }
            public object Instance => CreateObject();

            private readonly StandaloneTypeContainer typeContainer;
            private readonly Type type;
            private object singeltonInstance;

            public TypeInformation(StandaloneTypeContainer container, Type type, InstanceBehaviour instanceBehaviour)
            {
                this.type = type;
                Behaviour = instanceBehaviour;
                typeContainer = container;
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
