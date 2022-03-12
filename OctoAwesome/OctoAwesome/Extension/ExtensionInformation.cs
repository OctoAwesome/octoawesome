using dotVariant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Extension
{
    [Variant]
    public partial class ExtensionInformation
    {
        static partial void VariantOf(IExtension extension, IExtensionRegistrar registrar, IExtensionExtender extensionExtender);
    }
}
