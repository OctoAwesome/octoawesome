using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using OctoAwesome.Serialization;
using System.Buffers;
using System.IO.Compression;
using System.IO;

namespace OctoAwesome.GameServer
{
    public class PackageActionHub
    {

        public record struct RequestContext(BinaryReader Reader, Package Package)
        {
            public void SetResult<T>(T instance) where T : INoosonSerializable<T>
            {
                using var ms = Serializer.Manager.GetStream();
                using var bw = new BinaryWriter(ms);
                bw.Write(typeof(T).SerializationId());
                instance.Serialize(bw);
                Package.Payload = ms.ToArray();
                Package.PackageFlags &= ~(PackageFlags.Array | PackageFlags.Request);
                Package.PackageFlags |= PackageFlags.Response;
            }
            public void SetResult<T>(Span<T> instance) where T : INoosonSerializable<T>
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
        public void Register<T>(Action<ReadOnlyMemory<T>, RequestContext> action, ulong id = 0) where T : INoosonSerializable<T>, new()
        {
            if (id == 0)
                id = typeof(T).SerializationId();

            var deserializeAction = (RequestContext package) =>
            {
                var length = package.Reader.ReadInt32();
                var writeTo = ArrayPool<T>.Shared.Rent(length);
                for (int i = 0; i < length; i++)
                    writeTo[i] = T.Deserialize(package.Reader);

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
        public void Register<T>(Action<T, RequestContext> action, ulong id = 0) where T : INoosonSerializable<T>, new()
        {
            if (id == 0)
                id = typeof(T).SerializationId();

            var deserializeAction = (RequestContext package) =>
            {
                var t = T.Deserialize(package.Reader);
                action(t, package);
            };
            registeredStuff.Add(id, deserializeAction);
        }

        public void Dispatch(Package package, BaseClient client)
        {
            //TODO Not supported by nooson yet, so we don't pool for now, sadly
            //var pooled = typeof(IPoolElement).IsAssignableFrom(serType);
            //Serializer.DeserializePoolElement();

            using var ms = Serializer.Manager.GetStream(package.Payload);
            using Stream s = (package.PackageFlags & PackageFlags.Compressed) > 0 ? new GZipStream(ms, CompressionLevel.Optimal) : ms;
            using var br = new BinaryReader(s);

            var desId = br.ReadUInt64();

            var rc = new RequestContext(br, package);

            if ((package.PackageFlags & PackageFlags.Array) > 0)
                registeredStuffDic[desId].Invoke(rc);
            else
                registeredStuff[desId].Invoke(rc);

            if ((package.PackageFlags & PackageFlags.Response) > 0 
                && package.Payload is not null 
                && package.Payload.Length > 0)
                _ = client.SendPackageAndReleaseAsync(package);


        }
    }

}
