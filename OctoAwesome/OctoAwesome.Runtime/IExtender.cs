using System;
using System.Collections.Generic;

namespace OctoAwesome.Runtime;

public interface IExtender<TExtension, TExtend>
{
    //public IExtensionLoader<TExtend> ExtensionsTypes { get; set; }
    List<IExtensionLoader> ExtensionLoader { get; }
    Type ExtensionType { get; }
}