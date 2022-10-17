using dotVariant;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Extension
{
    /// <summary>
    /// Union type of <see cref="IExtension"/>, <see cref="IExtensionRegistrar"/> and <see cref="IExtensionExtender"/>
    /// </summary>
    [Variant]
    public partial class ExtensionInformation
    {
        static partial void VariantOf([NoImplicitConversion] IExtension extension, [NoImplicitConversion] IExtensionRegistrar registrar, [NoImplicitConversion] IExtensionExtender extensionExtender);
    }
}
