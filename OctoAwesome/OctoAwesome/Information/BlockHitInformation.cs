using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Information
{
    /// <summary>
    /// Represents information on block hits.
    /// </summary>
    public readonly struct BlockHitInformation : IEquatable<BlockHitInformation>
    {
        /// <summary>
        /// An empty default initialized instance.
        /// </summary>
        public static BlockHitInformation Empty = default;

        /// <summary>
        /// Gets a value indicating whether this instance equals <see cref="Empty"/>.
        /// </summary>
        public bool IsEmpty => !IsHitValid && Quantity == 0 && definitions == null;

        /// <summary>
        /// Gets a value indicating whether the hit was valid.
        /// </summary>
        public bool IsHitValid { get; }

        /// <summary>
        /// Gets the quantity that was harvested with this hit.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets an enumeration of the definitions that were mined from the hit and their corresponding quantity.
        /// </summary>
        public IReadOnlyList<(int Quantity, IDefinition Definition)> Definitions
            => definitions ?? Array.Empty<(int Quantity, IDefinition Definition)>();

        private readonly (int Quantity, IDefinition Definition)[] definitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockHitInformation"/> struct.
        /// </summary>
        /// <param name="isHitValid">A value indicating whether the hit was valid.</param>
        /// <param name="quantity">Gets the quantity that was harvested with this hit.</param>
        /// <param name="definitions">
        /// An enumeration of the definitions that were mined from the hit and their corresponding quantity.
        /// </param>
        public BlockHitInformation(bool isHitValid, int quantity, (int Quantity, IDefinition Definition)[] definitions)
        {
            IsHitValid = isHitValid;
            Quantity = quantity;
            this.definitions = definitions;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is BlockHitInformation information && Equals(information);

        /// <inheritdoc />
        public bool Equals(BlockHitInformation other)
            => IsHitValid == other.IsHitValid
                && Quantity == other.Quantity
                && EqualityComparer<(int Quantity, IDefinition Definition)[]>.Default.Equals(definitions, other.definitions);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = -1198439795;
            hashCode = hashCode * -1521134295 + IsHitValid.GetHashCode();
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<(int Quantity, IDefinition Definition)[]>.Default.GetHashCode(definitions);
            return hashCode;
        }

        /// <summary>
        /// Compares whether two <see cref="BlockHitInformation"/> structs are equal.
        /// </summary>
        /// <param name="left">The first block hit information to compare to.</param>
        /// <param name="right">The second block hit information to compare with.</param>
        /// <returns>A value indicating whether the two block hit information are equal.</returns>
        public static bool operator ==(BlockHitInformation left, BlockHitInformation right)
            => left.Equals(right);

        /// <summary>
        /// Compares whether two <see cref="BlockHitInformation"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first block hit information to compare to.</param>
        /// <param name="right">The second block hit information to compare with.</param>
        /// <returns>A value indicating whether the two block hit information are unequal.</returns>
        public static bool operator !=(BlockHitInformation left, BlockHitInformation right)
            => !(left == right);
    }
}
