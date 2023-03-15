using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NonSucking.Framework.Serialization;

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