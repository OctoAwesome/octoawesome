using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Extension;

public class ExtensionService
{

    private class ExtenderInformation
    {
        public List<Type> Types { get; } = new();
        public List<IExtensionLoader> Loader = new();

    }

    private List<ExtenderInformation> Extender = new();


    public void AddExtensionLoader(IExtensionLoader loader)
    {
        foreach (var t in loader.GetType().GetInterfaces())
        {
            if (!t.IsGenericType)
                continue;

            var genType = (t.IsGenericTypeDefinition ? t : t.GetGenericTypeDefinition());
            var isExtensionExtender = genType == typeof(IExtensionExtender<>);
            var isExtensionRegistrar = genType == typeof(IExtensionRegistrar<>);
            if (!isExtensionExtender && !isExtensionRegistrar)
                continue;

            var genArgs = t.GetGenericArguments();
            if (genArgs[0].IsGenericTypeParameter)
                throw new NotSupportedException("Currently not supported to have a generic extension loader");

            var genArg = genArgs[0];
            bool isAddedSomewhere = false;

            foreach (var ext in Extender)
            {
                foreach (var typ in ext.Types)
                {
                    if (typ == genArg)
                    {
                        ext.Loader.Add(loader);
                        isAddedSomewhere = true;
                        break;
                    }
                }
            }

            if (!isAddedSomewhere)
            {
                var extInf = new ExtenderInformation();
                extInf.Types.Add(genArgs[0]);
                extInf.Loader.Add(loader);
                Extender.Add(extInf);
            }
        }
    }


    //TODO Add Methods for string type name and Type without generic
    //TODO Return False or True based on success
    public void Extend<T>(Action<T> extend)
    {

    }

    public void Register<TRegister>(TRegister value)
    {

    }
    public void Unregister<TUnregister>(TUnregister value)
    {

    }

}
