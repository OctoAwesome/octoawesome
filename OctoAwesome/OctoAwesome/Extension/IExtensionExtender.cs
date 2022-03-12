﻿
using System;

namespace OctoAwesome.Extension
{
    public interface IExtensionExtender
    {

    }

    public interface IExtensionExtender<TExtensionType> : IExtensionExtender
    {
        void Execute<T>(T instance) where T : TExtensionType;
        void RegisterExtender<T>(Action<T> extenderDelegate) where T : TExtensionType;
    }
}
