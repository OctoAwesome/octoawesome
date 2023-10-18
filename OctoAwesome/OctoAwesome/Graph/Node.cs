using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Graph;
public abstract class Node
{
    public BlockInfo BlockInfo { get; set; }
    public Index3 Position => BlockInfo.Position;

    public abstract int Update(int state);

    public override string ToString()
    {
        return $"{BlockInfo.Position} {BlockInfo.Block} {BlockInfo.Meta}";
    }
}
