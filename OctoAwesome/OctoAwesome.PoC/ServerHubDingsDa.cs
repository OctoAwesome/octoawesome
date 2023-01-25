using OctoAwesome.Caching;
using OctoAwesome.Database;
using OctoAwesome.Network;
using OctoAwesome.Serialization;

using SixLabors.ImageSharp.Processing;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC;


public class Server
{
    private readonly CentralHub centralHub;
    private readonly JulianHub jHub;

    public void StartUp()
    {
        //centralHub.Register<A>(CommandHub.DoMagic);
        //centralHub.Register<A>(CommandHub.DoMagicWithList);
        //centralHub.Register<A>(CommandHub.DoMagic2);

        jHub.Register<List<A>>((a) => { });
        jHub.Register<A>((a) => { });
    }
}

public class CentralHub
{
    record struct RequestContext(BinaryReader Reader, Package Package/*, List<object> Responses*/);
    private readonly Dictionary<ulong, Action<RequestContext>> registeredStuff = new();
    private readonly Dictionary<ulong, Action<RequestContext>> registeredStuffDic = new();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
    internal void Register<T>(Action<ReadOnlyMemory<T>> action, ulong id = 0) where T : INoosonSerializable<T>, new()
    {
        if (id == 0)
            id = typeof(T).SerializationId();

        var deserializeAction = (RequestContext package) =>
        {
            var length = package.Reader.ReadInt32();
            var writeTo = ArrayPool<T>.Shared.Rent(length);
            for (int i = 0; i < length; i++)
                writeTo[i] = T.Deserialize(package.Reader);

            action(writeTo.AsMemory(0..length));
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
    internal void Register<T>(Action<T> action, ulong id = 0) where T : INoosonSerializable<T>, new()
    {
        if (id == 0)
            id = typeof(T).SerializationId();

        var deserializeAction = (RequestContext package) =>
        {
            var t = T.Deserialize(package.Reader);
            action(t);
        };
        registeredStuff.Add(id, deserializeAction);
    }

    internal void Deserialize(Package package)
    {
        using var ms = Serializer.Manager.GetStream(package.Payload);
        using Stream s = (package.PackageFlags & PackageFlags.Compressed) > 0 ? new GZipStream(ms, CompressionLevel.Optimal) : ms;
        using var br = new BinaryReader(s);

        var desId = br.ReadUInt64();

        if ((package.PackageFlags & PackageFlags.Array) > 0)
            registeredStuffDic[desId].Invoke(new RequestContext(br, package));
        else
            registeredStuff[desId].Invoke(new RequestContext(br, package));
    }
}

public class JulianHub
{
    Dictionary<ulong, Action<object>> register;

    public void Register<T>(Action<T> action)
    {
        register.Add(1L, GenericCaster<Action<T>, Action<object>>.Cast(action));
    }


    public void Push(Package package)
    {
        var typeId = BitConverter.ToUInt64(package.Payload);

        register.TryGetValue(typeId, out var dingensDa);
        //deserialize to object
        object o;
        //dingensDa.Invoke(o);
    }
}


public static class CommandHub
{
    public static void DoMagic(A request)
    {
    }
    public static void DoMagic2(A request)
    {

    }
    public static void DoMagicWithList(ReadOnlyMemory<A> request)
    {

    }
}

[SerializationId(0, 0)]
public class A : B
{

}
[SerializationId(0, 1)]
public class B
{

}