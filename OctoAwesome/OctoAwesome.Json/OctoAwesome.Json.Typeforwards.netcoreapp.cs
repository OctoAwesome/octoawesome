// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
global using System.Text;

// The compiler emits a reference to the internal copy of this type in our non-NETCoreApp assembly
// so we must include a forward to be compatible with libraries compiled against non-NETCoreApp OctoAwesome.Json
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Runtime.CompilerServices.IsExternalInit))]
