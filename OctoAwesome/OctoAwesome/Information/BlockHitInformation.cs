using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Information
{
    public readonly struct BlockHitInformation
    {
        public bool IsHitValid { get;  }
        public int Quantity { get; }
        public IReadOnlyList<KeyValuePair<int, IDefinition>> Definitions { get; }

        public BlockHitInformation(bool isHitValid, int quantity, KeyValuePair<int, IDefinition>[] definitions)
        {
            IsHitValid = isHitValid;
            Quantity = quantity;
            Definitions = definitions;
        }
    }
}
