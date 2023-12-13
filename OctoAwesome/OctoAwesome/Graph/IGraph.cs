using OctoAwesome.Serialization;

using System;
using System.IO;

namespace OctoAwesome.Graph;

//public interface IGraph
//{
//    int PlanetId { get; }
//    string TransferType { get; }

//    void AddBlock(BlockInfo info, Action<bool, BlockInfo> stateChangedCallback);
//    void RemoveNode(BlockInfo info);
//    void Update();
//    /// <summary>
//    /// Serialize this instance to a <see cref="BinaryWriter"/>.
//    /// </summary>
//    /// <param name="writer">The binary writer to write the serialized instance to.</param>
//    void Serialize(BinaryWriter writer);
//    /// <summary>
//    /// Deserialize this instance from a <see cref="BinaryReader"/>.
//    /// </summary>
//    /// <param name="reader">The binary reader to read the serialized instance from.</param>
//    static abstract IGraph Deserialize(BinaryReader reader);
//}