using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using OctoAwesome.Serialization;
using System.Buffers;
using System.IO.Compression;
using System.IO;
using OctoAwesome.Logging;
using NonSucking.Framework.Extension.IoC;
using OctoAwesome.Pooling;
using System.Reflection;
using OctoAwesome.Notifications;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using OctoAwesome.Caching;
using System.Text.Json.Serialization;

namespace OctoAwesome.GameServer
{
    public class PackageActionHub
    {
        private readonly ILogger logger;
        private readonly ITypeContainer typeContainer;
        private readonly SerializationIdTypeProvider typeRegistrar;
        private readonly IUpdateHub updateHub;
        private readonly Dictionary<ulong, Func<BinaryReader, object>> notificationDeserializationMethodCache;

        //private readonly Dictionary<long, >

        public PackageActionHub(ILogger logger, ITypeContainer tc)
        {
            this.logger = logger.As(nameof(PackageActionHub));
            this.typeContainer = tc;
            typeRegistrar = tc.Get<SerializationIdTypeProvider>();
            updateHub = tc.Get<IUpdateHub>();
            notificationDeserializationMethodCache = new();
        }

        public record struct RequestContext(BinaryReader Reader, Package Package)
        {
            public void SetResult<T>(T instance) where T : ISerializable<T>
            {
                using var ms = Serializer.Manager.GetStream();
                using var bw = new BinaryWriter(ms);
                bw.Write(typeof(T).SerializationId());
                instance.Serialize(bw);
                Package.Payload = ms.ToArray();
                Package.PackageFlags &= ~(PackageFlags.Array | PackageFlags.Request);
                Package.PackageFlags |= PackageFlags.Response;
            }
            public void SetResult<T>(Span<T> instance) where T : ISerializable<T>
            {
                using var ms = Serializer.Manager.GetStream();
                using var bw = new BinaryWriter(ms);
                bw.Write(typeof(T).SerializationId());
                bw.Write(instance.Length);
                foreach (var item in instance)
                    item.Serialize(bw);

                Package.Payload = ms.ToArray();
                Package.PackageFlags &= ~PackageFlags.Request;
                Package.PackageFlags |= (PackageFlags.Array | PackageFlags.Response);
            }

            public void SetResult(byte[] res)
            {
                Package.Payload = res;
                Package.PackageFlags &= ~PackageFlags.Request;
                Package.PackageFlags |= PackageFlags.Response;
            }

        }

        private readonly Dictionary<ulong, Action<RequestContext>> registeredStuff = new();
        private readonly Dictionary<ulong, Action<RequestContext>> registeredStuffDic = new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
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

        public void Dispatch(Package package, BaseClient client)
        {
            using var ms = Serializer.Manager.GetStream(package.Payload);
            using Stream s = (package.PackageFlags & PackageFlags.Compressed) > 0 ? new GZipStream(ms, CompressionLevel.Optimal) : ms;
            using var br = new BinaryReader(s);

            bool isNotification = (package.PackageFlags & PackageFlags.Notification) > 0;

            var desId = br.ReadUInt64();
            string channel = "";
            if (isNotification)
                channel = br.ReadString();
            var startOfObjectPos = ms.Position;

            var rc = new RequestContext(br, package);
            Action<RequestContext>? val = null;
            if ((package.PackageFlags & PackageFlags.Array) > 0 && registeredStuffDic.TryGetValue(desId, out val))
                val.Invoke(rc);
            else if ((package.PackageFlags & PackageFlags.Array) == 0 && registeredStuff.TryGetValue(desId, out val))
                val.Invoke(rc);

            if (isNotification)
            {
                ms.Seek(startOfObjectPos, SeekOrigin.Begin);

                if (!notificationDeserializationMethodCache.TryGetValue(desId, out var expression))
                {
                    var notificationType = typeRegistrar.Get(desId);
                    if (notificationType.IsAssignableTo(typeof(IPoolElement)))
                    {
                        var type = typeof(IPool<>).MakeGenericType(notificationType);
                        var objectPool = typeContainer.Get(type);
                    
                        dynamic pool = objectPool; //Todo, how to cast correctly to get access to rent?
                        //var pool = GenericCaster<object, IPool<IPoolElement>>.Cast(
                            //typeContainer.Get(type));
                        notificationDeserializationMethodCache[desId]
                            = expression
                            = (BinaryReader reader) =>
                            {
                                var element = pool.Rent();
                                var des = GenericCaster<IPoolElement, ISerializable>.Cast(element);
                                des.Deserialize(br);
                                return des;
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

                logger.Trace($"Sen: Package with id:{package.UId} and Flags: {package.PackageFlags}");
                _ = client.SendPackageAndReleaseAsync(package);
            }

            if (val is null)
            {
                logger.Trace($"Received invalid desId ({desId}) with flags: {package.PackageFlags}, send help");
            }
        }
    }

}
