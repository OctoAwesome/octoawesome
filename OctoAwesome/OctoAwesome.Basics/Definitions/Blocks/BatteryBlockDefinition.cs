using engenious.Graphics;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Graph;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Blocks;
/// <summary>
/// Block definition for light on blocks.
/// </summary>
public class BatteryBlockDefinition : BlockDefinition, INetworkBlock<int>
{
    /// <inheritdoc />
    public override string Icon => "light_on";

    /// <inheritdoc />
    public override string DisplayName => "Akku";

    /// <inheritdoc />
    public override string[] Textures { get; } = { "light_off", "light_on", "light_off", "light_on", "light_off", "light_on", "light_off", "light_on" };

    /// <inheritdoc />
    public override IMaterialDefinition Material { get; }
    public string TransferType => "Energy";

    /// <summary>
    /// Initializes a new instance of the <see cref="CactusBlockDefinition"/> class.
    /// </summary>
    /// <param name="material">The material definition for this cactus block definition.</param>
    public BatteryBlockDefinition(CactusMaterialDefinition material)
    {
        Material = material;
    }

    /// <inheritdoc/>
    public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
    {
        var meta = manager.GetBlockMeta(x, y, z);
        return meta & 7;
    }

    public Node<int> CreateNode()
    {
        return new BatteryNode();
    }
}





internal partial class BatteryNode : Node<int>, ITargetNode<int>, ISourceNode<int>
{
    const int MaxCharge = 30000;
    const int MaxChargeRate = 100;
    const int MaxDischargeRate = 500;

    int currentCharge = 0;
    private int canChargeThisRound;

    int ITargetNode<int>.Priority { get; } = int.MaxValue;
    int ISourceNode<int>.Priority { get; } = int.MaxValue;

    public SourceInfo<int> GetCapacity()
    {
        currentCharge = Math.Min(MaxCharge, Math.Max(0, currentCharge));

        return new SourceInfo<int>(default, default) { Node = this, Data = Math.Min(MaxDischargeRate, currentCharge) };
    }

    public TargetInfo<int> GetRequired()
    {
        canChargeThisRound = Math.Max(0, Math.Min(MaxCharge - currentCharge, MaxChargeRate));

        return new(this, 1, canChargeThisRound, 0);
    }

    public void Use(SourceInfo<int> sourceInfo, IChunkColumn column)
    {
        currentCharge -= sourceInfo.UseInfo;
        column.SetBlockMeta(Position, currentCharge / 3750);
    }

    public void Execute(TargetInfo<int> targetInfo, IChunkColumn column)
    {
        currentCharge += targetInfo.RepeatedTimes;
        column.SetBlockMeta(Position, currentCharge / 3750);

    }

    public override void Deserialize(BinaryReader reader)
    {
        base.Deserialize(reader);
        currentCharge = reader.ReadInt32();
    }

    public override void Serialize(BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(currentCharge);
    }

}