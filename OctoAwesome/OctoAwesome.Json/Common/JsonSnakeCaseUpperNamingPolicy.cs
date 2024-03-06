// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace OctoAwesome.Json
{
    internal sealed class JsonSnakeCaseUpperNamingPolicy : JsonSeparatorNamingPolicy
    {
        public JsonSnakeCaseUpperNamingPolicy()
            : base(lowercase: false, separator: '_')
        {
        }
    }
}
