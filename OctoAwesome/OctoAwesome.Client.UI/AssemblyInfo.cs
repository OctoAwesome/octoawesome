using OctoAwesome;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: NoosonConfiguration(
    GenerateDeserializeExtension = false,
    DisableWarnings = true,
    GenerateStaticDeserializeWithCtor = true,
    GenerateDeserializeOnInstance = true,
    GenerateStaticSerialize = true,
    GenerateStaticDeserializeIntoInstance = true,
    NameOfStaticDeserializeWithCtor = "DeserializeAndCreate",
    NameOfDeserializeOnInstance = "Deserialize",
    NameOfStaticDeserializeIntoInstance = "Deserialize",
    NameOfStaticDeserializeWithOutParams = "DeserializeOut")]

[assembly: SerializationId<UiKeyComponent>(3, 1)]
[assembly: SerializationId<UiMappingComponent>(3, 2)]