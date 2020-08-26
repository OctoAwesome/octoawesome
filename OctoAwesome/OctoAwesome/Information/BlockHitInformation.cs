using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Information
{
    public readonly struct BlockHitInformation : IEquatable<BlockHitInformation>
    {
        public static BlockHitInformation Empty = default;

        public bool IsEmpty => !IsHitValid && Quantity == 0 && definitions == null;

        public bool IsHitValid { get;  }
        public int Quantity { get; }
        public IReadOnlyList<(int Quantity, IDefinition Definition)> Definitions
            => definitions ?? Array.Empty<(int Quantity, IDefinition Definition)>();

        private readonly (int Quantity, IDefinition Definition)[] definitions;

        public BlockHitInformation(bool isHitValid, int quantity, (int Quantity, IDefinition Definition)[] definitions)
        {
            IsHitValid = isHitValid;
            Quantity = quantity;
            this.definitions = definitions;
        }

        public override bool Equals(object obj) 
            => obj is BlockHitInformation information && Equals(information);
        public bool Equals(BlockHitInformation other) 
            => IsHitValid == other.IsHitValid 
                && Quantity == other.Quantity 
                && EqualityComparer<(int Quantity, IDefinition Definition)[]>.Default.Equals(definitions, other.definitions);

        public override int GetHashCode()
        {
            var hashCode = -1198439795;
            hashCode = hashCode * -1521134295 + IsHitValid.GetHashCode();
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<(int Quantity, IDefinition Definition)[]>.Default.GetHashCode(definitions);
            return hashCode;
        }

        public static bool operator ==(BlockHitInformation left, BlockHitInformation right) 
            => left.Equals(right);
        public static bool operator !=(BlockHitInformation left, BlockHitInformation right) 
            => !(left == right);
    }
}
