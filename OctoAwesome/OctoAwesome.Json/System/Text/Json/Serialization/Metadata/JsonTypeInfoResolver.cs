﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace OctoAwesome.Json.Serialization.Metadata
{
    /// <summary>
    /// Contains utilities and combinators acting on <see cref="IJsonTypeInfoResolver"/>.
    /// </summary>
    public static class JsonTypeInfoResolver
    {
        /// <summary>
        /// Combines multiple <see cref="IJsonTypeInfoResolver"/> sources into one.
        /// </summary>
        /// <param name="resolvers">Sequence of contract resolvers to be queried for metadata.</param>
        /// <returns>A <see cref="IJsonTypeInfoResolver"/> combining results from <paramref name="resolvers"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="resolvers"/> is null.</exception>
        /// <remarks>
        /// The combined resolver will query each of <paramref name="resolvers"/> in the specified order,
        /// returning the first result that is non-null. If all <paramref name="resolvers"/> return null,
        /// then the combined resolver will also return <see langword="null"/>.
        ///
        /// Can be used to combine multiple <see cref="JsonSerializerContext"/> sources,
        /// which typically define contract metadata for small subsets of types.
        /// It can also be used to fall back to <see cref="DefaultJsonTypeInfoResolver"/> wherever necessary.
        /// </remarks>
        public static IJsonTypeInfoResolver Combine(params IJsonTypeInfoResolver?[] resolvers)
        {
            if (resolvers is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(resolvers));
            }

            return Combine((ReadOnlySpan<IJsonTypeInfoResolver?>)resolvers);
        }

        /// <summary>
        /// Combines multiple <see cref="IJsonTypeInfoResolver"/> sources into one.
        /// </summary>
        /// <param name="resolvers">Sequence of contract resolvers to be queried for metadata.</param>
        /// <returns>A <see cref="IJsonTypeInfoResolver"/> combining results from <paramref name="resolvers"/>.</returns>
        /// <remarks>
        /// The combined resolver will query each of <paramref name="resolvers"/> in the specified order,
        /// returning the first result that is non-null. If all <paramref name="resolvers"/> return null,
        /// then the combined resolver will also return <see langword="null"/>.
        ///
        /// Can be used to combine multiple <see cref="JsonSerializerContext"/> sources,
        /// which typically define contract metadata for small subsets of types.
        /// It can also be used to fall back to <see cref="DefaultJsonTypeInfoResolver"/> wherever necessary.
        /// </remarks>
        public static IJsonTypeInfoResolver Combine(/*params*/ ReadOnlySpan<IJsonTypeInfoResolver?> resolvers)
        {
            var resolverChain = new JsonTypeInfoResolverChain();
            foreach (IJsonTypeInfoResolver? resolver in resolvers)
            {
                resolverChain.AddFlattened(resolver);
            }

            return resolverChain.Count == 1 ? resolverChain[0] : resolverChain;
        }

        /// <summary>
        /// Creates a resolver applies modifications to the metadata generated by the source <paramref name="resolver"/>.
        /// </summary>
        /// <param name="resolver">The source resolver generating <see cref="JsonTypeInfo"/> metadata.</param>
        /// <param name="modifier">The delegate modifying non-null <see cref="JsonTypeInfo"/> results.</param>
        /// <returns>A new <see cref="IJsonTypeInfoResolver"/> instance applying the modifications.</returns>
        /// <remarks>
        /// This method is closely related to <see cref="DefaultJsonTypeInfoResolver.Modifiers"/> property
        /// extended to arbitrary <see cref="IJsonTypeInfoResolver"/> instances.
        /// </remarks>
        public static IJsonTypeInfoResolver WithAddedModifier(this IJsonTypeInfoResolver resolver, Action<JsonTypeInfo> modifier)
        {
            if (resolver is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(resolver));
            }
            if (modifier is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(modifier));
            }

            return resolver is JsonTypeInfoResolverWithAddedModifiers resolverWithModifiers
                ? resolverWithModifiers.WithAddedModifier(modifier)
                : new JsonTypeInfoResolverWithAddedModifiers(resolver, new[] { modifier });
        }

        /// <summary>
        /// Gets a resolver that returns null <see cref="JsonTypeInfo"/> for every type.
        /// </summary>
        internal static IJsonTypeInfoResolver Empty { get; } = new EmptyJsonTypeInfoResolver();

        /// <summary>
        /// Indicates whether the metadata generated by the current resolver
        /// are compatible with the run time specified <see cref="JsonSerializerOptions"/>.
        /// </summary>
        internal static bool IsCompatibleWithOptions(this IJsonTypeInfoResolver? resolver, JsonSerializerOptions options)
            => resolver is IBuiltInJsonTypeInfoResolver bir && bir.IsCompatibleWithOptions(options);
    }

    /// <summary>
    /// A <see cref="IJsonTypeInfoResolver"/> that returns null for all inputs.
    /// </summary>
    internal sealed class EmptyJsonTypeInfoResolver : IJsonTypeInfoResolver, IBuiltInJsonTypeInfoResolver
    {
        public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options) => null;
        public bool IsCompatibleWithOptions(JsonSerializerOptions _) => true;
    }

    /// <summary>
    /// Implemented by the built-in converters to avoid rooting
    /// unused resolver dependencies in the context of the trimmer.
    /// </summary>
    internal interface IBuiltInJsonTypeInfoResolver
    {
        /// <summary>
        /// Indicates whether the metadata generated by the current resolver
        /// are compatible with the run time specified <see cref="JsonSerializerOptions"/>.
        /// </summary>
        bool IsCompatibleWithOptions(JsonSerializerOptions options);
    }
}
