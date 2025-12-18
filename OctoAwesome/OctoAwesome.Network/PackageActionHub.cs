using System;
using System.Collections.Generic;
using OctoAwesome.Serialization;
using System.Buffers;
using System.IO.Compression;
using System.IO;
using OctoAwesome.Logging;
using OctoAwesome.Pooling;
using System.Reflection;
using OctoAwesome.Notifications;
using System.Linq.Expressions;
using OctoAwesome.Caching;

namespace OctoAwesome.Network
{
    /// <summary>
    /// The hub where all package actions can be registered.
    /// </summary>
    public class PackageActionHub
    {
        private readonly ILogger logger;
        private readonly ITypeContainer typeContainer;
        private readonly IUpdateHub updateHub;
        private readonly Dictionary<ulong, Func<BinaryReader, object>> notificationDeserializationMethodCache;

        private readonly Dictionary<ulong, Action<RequestContext>> registeredStuff = new();
        private readonly Dictionary<ulong, Action<RequestContext>> registeredStuffDic = new();

        /// <summary>
        /// Initializes a new instance of the package action hub.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="tc">The current used type container</param>
        public PackageActionHub(ILogger logger, ITypeContainer tc)
        {
            this.logger = logger.As(nameof(PackageActionHub));
            typeContainer = tc;
            updateHub = tc.Get<IUpdateHub>();
            notificationDeserializationMethodCache = new();
        }

        /// <summary>
        /// Registers an action for a non-poolable type that can use ReadOnlyMemory optimizations.
        /// </summary>
        /// <typeparam name="T">The type that should be deserialized</typeparam>
        /// <param name="action">The action that gets invoked after deserialization</param>
        /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
        public void Register<T>(Action<ReadOnlyMemory<T>, RequestContext> action, ulong id = 0) where T : IConstructionSerializable<T>
        {
            if (id == 0)
                id = typeof(T).SerializationId();

            var deserializeAction = (RequestContext package) =>
            {
                var length = package.Reader.ReadInt32();
                var writeTo = ArrayPool<T>.Shared.Rent(length);
                for (int i = 0; i < length; i++)
                    writeTo[i] = T.DeserializeAndCreate(package.Reader);

                action(writeTo.AsMemory(0..length), package);
                ArrayPool<T>.Shared.Return(writeTo);
            };
            registeredStuffDic.Add(id, deserializeAction);
        }

        /// <summary>
        /// Registers an action for a poolable type that can use ReadOnlyMemory optimizations.
        /// </summary>
        /// <typeparam name="T">The type that should be deserialized</typeparam>
        /// <param name="action">The action that gets invoked after deserialization</param>
        /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
        public void RegisterPoolable<T>(Action<ReadOnlyMemory<T>, RequestContext> action, ulong id = 0) where T : IConstructionSerializable<T>, IPoolElement
        {
            if (id == 0)
                id = typeof(T).SerializationId();
            var pool = typeContainer.Get<IPool<T>>();

            var deserializeAction = (RequestContext package) =>
            {
                var length = package.Reader.ReadInt32();
                var writeTo = ArrayPool<T>.Shared.Rent(length);
                for (int i = 0; i < length; i++)
                {
                    var t = pool.Rent();
                    t.Deserialize(package.Reader);
                    writeTo[i] = t;
                }

                action(writeTo.AsMemory(0..length), package);
                foreach (var item in writeTo)
                    item.Release();

                ArrayPool<T>.Shared.Return(writeTo);
            };
            registeredStuffDic.Add(id, deserializeAction);
        }

        /// <summary>
        /// Registers an action for a non-poolable type.
        /// </summary>
        /// <typeparam name="T">The type that should be deserialized</typeparam>
        /// <param name="action">The action that gets invoked after deserialization</param>
        /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
        public void Register<T>(Action<T, RequestContext> action, ulong id = 0) where T : IConstructionSerializable<T>
        {
            if (id == 0)
                id = typeof(T).SerializationId();

            var deserializeAction = (RequestContext package) =>
            {
                var t = T.DeserializeAndCreate(package.Reader);
                action(t, package);
            };
            registeredStuff.Add(id, deserializeAction);
        }

        /// <summary>
        /// Registers an action for a poolable type.
        /// </summary>
        /// <typeparam name="T">The type that should be deserialized</typeparam>
        /// <param name="action">The action that gets invoked after deserialization</param>
        /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
        public void RegisterPoolable<T>(Action<T, RequestContext> action, ulong id = 0) where T : IConstructionSerializable<T>, IPoolElement
        {
            if (id == 0)
                id = typeof(T).SerializationId();
            var pool = typeContainer.Get<IPool<T>>();
            var deserializeAction = (RequestContext package) =>
            {
                var t = pool.Rent();
                t.Deserialize(package.Reader);
                action(t, package);
                t.Release();
            };
            registeredStuff.Add(id, deserializeAction);
        }

        /// <summary>
        /// Registers an action for a poolable type without a RequestContext.
        /// </summary>
        /// <typeparam name="T">The type that should be deserialized</typeparam>
        /// <param name="action">The action that gets invoked after deserialization</param>
        /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
        public void RegisterPoolable<T>(Action<T> action, ulong id = 0) where T : IConstructionSerializable<T>, IPoolElement
        {
            if (id == 0)
                id = typeof(T).SerializationId();
            var pool = typeContainer.Get<IPool<T>>();
            var deserializeAction = (RequestContext package) =>
            {
                var t = pool.Rent();
                t.Deserialize(package.Reader);
                action(t);
                t.Release();
            };
            registeredStuff.Add(id, deserializeAction);
        }

        /// <summary>
        /// Dispatches a package to the appropriate registered action or notification handler.
        /// </summary>
        /// <param name="package">The package that should be dispatched</param>
        /// <param name="client">The client to dispatch to</param>
        public void Dispatch(Package package, BaseClient client)
        {
            var packageId = package.UId;
            logger.Trace($"Entered Dispatch logic for {packageId}");
            using var ms = Serializer.Manager.GetStream(package.Payload);
            using Stream s = (package.PackageFlags & PackageFlags.Compressed) > 0 ? new GZipStream(ms, CompressionLevel.Optimal, leaveOpen: true) : ms;
            using var br = new BinaryReader(s, System.Text.Encoding.Default, leaveOpen: true);

            bool isNotification = (package.PackageFlags & PackageFlags.Notification) > 0;

            var desId = br.ReadUInt64();
            string channel = "";
            if (isNotification)
                channel = br.ReadString();
            var startOfObjectPos = ms.Position;
            logger.Trace($"Got {(isNotification ? "Notification" : "Package")} with des id (Mod:{desId >> 32}, Type:{desId & 0xFFFFFFF}) {(isNotification ? $"for channel {channel}" : "")} package {packageId}");
            if (channel == "planet")
                ;
            var rc = new RequestContext(br, package);
            Action<RequestContext>? val = null;
            if ((package.PackageFlags & PackageFlags.Array) > 0 && registeredStuffDic.TryGetValue(desId, out val))
                val.Invoke(rc);
            else if ((package.PackageFlags & PackageFlags.Array) == 0 && registeredStuff.TryGetValue(desId, out val))
                val.Invoke(rc);

            logger.Trace($"All invocations successful, now dispatching to updatehub or sending response to client for {packageId}");

            if (isNotification)
            {
                ms.Seek(startOfObjectPos, SeekOrigin.Begin);

                if (!notificationDeserializationMethodCache.TryGetValue(desId, out var expression))
                {
                    var notificationType = SerializationIdTypeProvider.Get(desId);
                    if (notificationType.IsAssignableTo(typeof(IPoolElement)))
                    {
                        var type = typeof(IPool<>).MakeGenericType(notificationType);
                        var objectPool = typeContainer.Get(type);

                        var pool = GenericCaster<object, IPool>.Cast(objectPool);

                        notificationDeserializationMethodCache[desId]
                            = expression
                            = (BinaryReader reader) =>
                            {
                                var element = pool.RentElement();
                                if (element is ISerializable des)
                                {
                                    try
                                    {
                                        des.Deserialize(reader);
                                    }
                                    catch (Exception ex)
                                    {

                                        throw;
                                    }
                                    return des;
                                }
                                return default!;
                            };
                    }
                    else
                    {
                        var brParam = Expression.Parameter(typeof(BinaryReader));
                        MethodInfo deserializationMethodInfo;
                        deserializationMethodInfo = notificationType.GetMethod(nameof(IConstructionSerializable<bool>.DeserializeAndCreate), BindingFlags.Public | BindingFlags.Static, new[] { typeof(BinaryReader) })!;
                        notificationDeserializationMethodCache[desId]
                            = expression
                            = Expression.Lambda<Func<BinaryReader, object>>(Expression.Call(deserializationMethodInfo, brParam), brParam).Compile();
                    }
                    logger.Trace($"Cached expression for {packageId}");
                }

                var notification = expression(br);
                updateHub.Push(notification, channel);

                if (notification is IPoolElement poolElement)
                    poolElement.Release();
            }
            else if (val is not null
                && (package.PackageFlags & PackageFlags.Response) > 0
                && package.Payload is not null
                && package.Payload.Length > 0)
            {

                logger.Trace($"Sen: Package with id:{packageId} and Flags: {package.PackageFlags}");
                _ = client.SendPackageAndReleaseAsync(package);
            }

            if (val is null && !isNotification)
            {
                logger.Warn($"Received invalid desId ({desId}) with flags: {package.PackageFlags}, send help");
            }

            logger.Trace($"Finished Dispatch logic for {packageId}");
        }
    }

    /// <summary>
    /// Represents the context of a request, including the binary reader and the package.
    /// </summary>
    /// <param name="Reader">The binary reader used to read the package data.</param>
    /// <param name="Package">The package associated with the request.</param>
    public record struct RequestContext(BinaryReader Reader, Package Package)
    {
        /// <summary>
        /// Sets the result of the request context with a single instance of a serializable type.
        /// </summary>
        /// <typeparam name="T">The type of the instance to set as the result.</typeparam>
        /// <param name="instance">The instance to set as the result.</param>
        public void SetResult<T>(T instance) where T : ISerializable<T>
        {
            using var ms = Serializer.Manager.GetStream();
            using var bw = new BinaryWriter(ms, System.Text.Encoding.Default, leaveOpen: true);
            bw.Write(typeof(T).SerializationId());
            instance.Serialize(bw);
            Package.Payload = ms.ToArray();
            Package.PackageFlags &= ~(PackageFlags.Array | PackageFlags.Request);
            Package.PackageFlags |= PackageFlags.Response;
        }

        /// <summary>
        /// Sets the result of the request context with a span of serializable instances.
        /// </summary>
        /// <typeparam name="T">The type of the instances to set as the result.</typeparam>
        /// <param name="instance">The span of instances to set as the result.</param>
        public void SetResult<T>(Span<T> instance) where T : ISerializable<T>
        {
            using var ms = Serializer.Manager.GetStream();
            using var bw = new BinaryWriter(ms, System.Text.Encoding.Default, leaveOpen: true);
            bw.Write(typeof(T).SerializationId());
            bw.Write(instance.Length);
            foreach (var item in instance)
                item.Serialize(bw);

            Package.Payload = ms.ToArray();
            Package.PackageFlags &= ~PackageFlags.Request;
            Package.PackageFlags |= (PackageFlags.Array | PackageFlags.Response);
        }

        /// <summary>
        /// Sets the result of the request context with a byte array.
        /// </summary>
        /// <param name="res">The byte array to set as the result.</param>
        public void SetResult(byte[] res)
        {
            Package.Payload = res;
            Package.PackageFlags &= ~PackageFlags.Request;
            Package.PackageFlags |= PackageFlags.Response;
        }
    }


}
