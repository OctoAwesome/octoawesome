﻿
using System;
using System.IO;
using OctoAwesome.Extension;
using OctoAwesome.Serialization;

namespace OctoAwesome.Location
{
    /// <summary>
    /// A universe of OctoAwesome. A universe contains multiple planets and is a save state.
    /// </summary>
    [Nooson]
    public partial class Universe : IUniverse, IConstructionSerializable<Universe>
    {
        private string? name;

        /// <inheritdoc />
        public Guid Id { get; set; }

        /// <inheritdoc />
        public string Name
        {
            get => NullabilityHelper.NotNullAssert(name);
            set => name = NullabilityHelper.NotNullAssert(value, nameof(value));
        }

        /// <inheritdoc />
        public int Seed { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Universe"/> class.
        /// </summary>
        public Universe()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Universe"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> of the universe.</param>
        /// <param name="name">The name of the universe.</param>
        /// <param name="seed">The seed for generating the universe.</param>
        public Universe(Guid id, string name, int seed)
        {
            Id = id;
            Name = name;
            Seed = seed;
        }



    }
}