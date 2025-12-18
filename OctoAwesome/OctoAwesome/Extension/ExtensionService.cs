using Microsoft.Win32;

using OctoAwesome.Database;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Extension;

/// <summary>
/// The Service to manage extension registrars and extender
/// </summary>
public class ExtensionService
{

    private class ExtenderInformation
    {
        public List<IExtensionRegistrar> Registrars = new();
        public List<IExtensionExtender> Extenders = new();

    }

    private readonly Dictionary<Type, ExtenderInformation> extender = new();

    /// <summary>
    /// Adds the extensions, registrars or extender to this service
    /// </summary>
    /// <param name="extensionInformation">The Information to be added</param>
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
    /// <summary>
    /// The action that should be called on the instance to be extended
    /// </summary>
    /// <typeparam name="T">The type of the object to be extended</typeparam>
    /// <param name="extend">The action that will be called with the instance of the <typeparamref name="T"/></param>
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
                var extenderType = extender.GetType();
                var method = extenderType.GetMethod("RegisterExtender");
                Debug.Assert(method != null, $"RegisterExtender method not found on {extenderType}!");
                var genMethod = method.MakeGenericMethod(typeof(T));

                genMethod.Invoke(extender, [extend]);
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

    /// <summary>
    /// Executed all extender on the instance, so it's fully initialized afterwards
    /// </summary>
    /// <typeparam name="T">The type of the instance to be extended</typeparam>
    /// <param name="toExtend">The instance to extend</param>
    public void ExecuteExtender<T>(T toExtend)
    {
        Debug.Assert(toExtend != null, nameof(toExtend) + " != null");
        foreach (var key in GetAllBaseTypesAndInterfaces(typeof(T)))
        {
            if (!extender.TryGetValue(key, out var extenderInformation))
            {
                continue;
            }

            foreach (var extender in extenderInformation.Extenders)
            {
                var extenderType = extender.GetType();
                var method = extenderType.GetMethod("Execute");
                Debug.Assert(method != null, $"RegisterExtender method not found on {extenderType}!");
                var genMethod = method.MakeGenericMethod(typeof(T));

                genMethod.Invoke(extender, [toExtend]);
            }
        }
    }

    /// <summary>
    /// Register a registrar for this service
    /// </summary>
    /// <typeparam name="TRegister">The type of the registrar to be registered</typeparam>
    /// <param name="value">The instance of the registrar to be registered</param>
    /// <param name="channelName">The specific channel for the registrar, can be empty to have all channels matched</param>
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

    /// <summary>
    /// Unregister a registrar for this service
    /// </summary>
    /// <typeparam name="TUnregister">The type of the registrar to be unregistered</typeparam>
    /// <param name="value">The instance of the registrar to be unregistered</param>
    /// <param name="channelName">The specific channel for the registrar, can be empty to have all channels matched</param>
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

    /// <summary>
    /// Gets all registrar that are registered in this service with optionally search param for the channel name
    /// </summary>
    /// <param name="channelName">The channel name to filter, or empty if all channel names should be matched</param>
    /// <returns>All registered registrars that match the channel name or all registrars if the channel name is empty</returns>
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


    /// <summary>
    /// Gets all instances added in the specific registrar with optionally search param for the channel name
    /// </summary>
    /// <param name="channelName">The channel name to filter, or empty if all channel names should be matched</param>
    /// <returns>Enumerable of instances that are in the registrars</returns>
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

    public void RegisterTypesWithSerializationId(Assembly assembly)
    {
        var types = assembly.GetCustomAttributes<BaseSerializationIdAttribute>();

        foreach (var serIdAttribute in types)
        {

            var serId = serIdAttribute.CombinedId;

            if (serId > 0)
            {
                Register(serIdAttribute, ChannelNames.Serialization);
            }
        }
    }
}
