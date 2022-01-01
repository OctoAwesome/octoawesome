using Microsoft.Win32;

using OctoAwesome.Database;

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
        public List<IExtensionRegistrar> Registrars = new();
        public List<IExtensionExtender> Extenders = new();

    }

    private readonly Dictionary<Type, ExtenderInformation> extender = new();

    public void AddExtensionLoader(ExtensionInformation extensionInformation)
    {
        extensionInformation
            .Visit(
                AddExtension,
                AddRegistrar,
                AddExtender
            );
    }


    private void AddExtension(IExtension extension)
    {

    }

    private void AddRegistrar(IExtensionRegistrar registrar)
    {
        GetExtenderInformation(registrar.GetType(), typeof(IExtensionRegistrar<>).Name)
            .Registrars
            .Add(registrar);

    }

    private void AddExtender(IExtensionExtender extender)
    {
        GetExtenderInformation(extender.GetType(), typeof(IExtensionExtender<>).Name)
            .Extenders
            .Add(extender);
    }

    private ExtenderInformation GetExtenderInformation(Type instanceType, string interfaceName)
    {
        var registarInterfaceType = instanceType.GetInterface(interfaceName);

        if (registarInterfaceType is null)
            throw new ArgumentException($"Type dosen't implement {interfaceName}");

        var genArgs = registarInterfaceType.GetGenericArguments();
        if (genArgs[0].IsGenericTypeParameter)
            throw new NotSupportedException("Currently not supported to have a generic extension loader");

        var genericTypeArgument = genArgs[0];

        if (!extender.TryGetValue(genericTypeArgument, out var extenderInformation))
        {
            extenderInformation = new ExtenderInformation();
            extender.Add(genericTypeArgument, extenderInformation);
        }

        return extenderInformation;
    }

    //TODO Add Methods for string type name and Type without generic
    //TODO Return False or True based on success
    public void Extend<T>(Action<T> extend)
    {
        foreach (var key in GetAllBaseTypesAndInterfaces(typeof(T)))
        {
            if (!extender.TryGetValue(key, out var extenderInformation))
            {
                continue;
            }

            foreach (var extender in extenderInformation.Extenders)
            {
                if (extender is IExtensionExtender genericExtender)
                {
                    var method = extender.GetType().GetMethod("RegisterExtender");
                    var genMethod = method.MakeGenericMethod(typeof(T));

                    genMethod.Invoke(extender, new object[] { extend });
                }
            }
        }
    }

    private IEnumerable<Type> GetAllBaseTypesAndInterfaces(Type t)
    {
        yield return t;
        if (t.BaseType != null)
        {
            foreach (var item in GetAllBaseTypesAndInterfaces(t.BaseType))
            {
                yield return item;
            }
        }
        foreach (var item in t.GetInterfaces())
        {
            yield return item;
        }
    }

    public void ExecuteExtender<T>(T toExtend)
    {
        foreach (var key in GetAllBaseTypesAndInterfaces(typeof(T)))
        {
            if (!extender.TryGetValue(key, out var extenderInformation))
            {
                continue;
            }

            foreach (var extender in extenderInformation.Extenders)
            {
                if (extender is IExtensionExtender genericExtender)
                {
                    var method = extender.GetType().GetMethod("Execute");
                    var genMethod = method.MakeGenericMethod(typeof(T));

                    genMethod.Invoke(extender, new object[] { toExtend });
                }
            }
        }

    }

    public void Register<TRegister>(TRegister value, string channelName = "")
    {
        var key = typeof(TRegister);
        if (!extender.TryGetValue(key, out var extenderInformation))
        {
            return;
        }

        foreach (var extender in extenderInformation.Registrars)
        {
            if (extender is IExtensionRegistrar<TRegister> genericExtender
                && (string.IsNullOrEmpty(extender.ChannelName)
                    || string.IsNullOrEmpty(channelName)
                    || extender.ChannelName == channelName)
                )
            {
                genericExtender.Register(value);
            }
        }
    }

    public void Unregister<TUnregister>(TUnregister value, string channelName = "")
    {
        var key = typeof(TUnregister);
        if (!extender.TryGetValue(key, out var extenderInformation))
        {
            return;
        }

        foreach (var extender in extenderInformation.Registrars)
        {
            if (extender is IExtensionRegistrar<TUnregister> genericExtender
                && (string.IsNullOrEmpty(extender.ChannelName)
                    || string.IsNullOrEmpty(channelName)
                    || extender.ChannelName == channelName)
                )
            {
                genericExtender.Unregister(value);
            }
        }
    }

    public IEnumerable<IExtensionRegistrar> GetRegistrars(string channelName = "")
    {
        foreach (var item in extender)
        {
            foreach (var registrar in item.Value.Registrars)
            {
                if (string.IsNullOrEmpty(registrar.ChannelName)
                            || string.IsNullOrEmpty(channelName)
                            || registrar.ChannelName == channelName)
                {
                    yield return registrar;
                }

            }
        }
    }

    public IEnumerable<T> GetFromRegistrar<T>(string channelName = "")
    {
        if (!extender.TryGetValue(typeof(T), out var extenderInformation))
        {
            yield break;
        }

        foreach (var registrar in extenderInformation.Registrars)
        {
            if (registrar is IExtensionRegistrar<T> genericExtender
                && (string.IsNullOrEmpty(registrar.ChannelName)
                        || string.IsNullOrEmpty(channelName)
                        || registrar.ChannelName == channelName))
            {
                foreach (var item in genericExtender.Get())
                {
                    yield return item;
                }
            }

        }
    }

}
