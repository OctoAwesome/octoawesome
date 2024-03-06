// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace OctoAwesome.Json
{
    internal sealed class JsonSnakeCaseLowerNamingPolicy : JsonSeparatorNamingPolicy
    {
        public JsonSnakeCaseLowerNamingPolicy()
            : base(lowercase: true, separator: '_')
        {
        }
    }
}
