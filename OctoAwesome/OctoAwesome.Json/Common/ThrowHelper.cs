// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    internal static partial class ThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowArgumentNullException(string parameterName)
        {
            throw new ArgumentNullException(parameterName);
        }

        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowOutOfMemoryException_BufferMaximumSizeExceeded(uint capacity)
        {
            throw new OutOfMemoryException();
        }
    }
}
