using System;
using System.Collections.Generic;

namespace OctoAwesome.Runtime;

public interface IExtender<TExtension, TExtend>
{
    //public IExtensionLoader<TExtend> ExtensionsTypes { get; set; }
    List<IExtensionLoader> ExtensionLoader { get; }
    Type ExtensionType { get; }
}

/*
 * Mehrere Loader können einen Type Extenden / Registrieren
 * Können Loader immer nur einen Type? Bsp. DefinitionRegistrar nur IDefinition
 * 
1.
//Nutzung unschön, weil 3 Iterationen
//Woher weiß der Extender was die Extension loader laden können?
//Service in Extension geben, der dann die Registrars und Extender verwaltet
List<IExtender>
IExtender{
    List<Type>
    List<IExtensionLoader>
}

2.
//Schwierig wenn mehrer base Types, bsp. IBlockDefinition, IDefinition, BlockDefinition => Alles der gleiche Extensionloader
One IExtender{
   Dictionary<Type, List<IExtensionLoader>>
}
BlockDefinition
IBlockDefinition
IDefinition

3. 
//Types müssten ggf. mehrfach registriert werden, siehe 2.
Typecontainer
*/