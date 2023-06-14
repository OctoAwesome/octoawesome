using engenious;
using engenious.Graphics;

using OctoAwesome.Components;

using System;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for animated models.
    /// </summary>
    [Nooson]
    public partial class AnimationComponent : Component, IEntityComponent
    {

        /// <summary>
        /// Gets or sets the currently elapsed time for the animation.
        /// </summary>
        public float CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum time the animation should take.
        /// </summary>
        public float MaxTime { get; set; }

        /// <summary>
        /// Gets or sets the speed the animation should be played with.
        /// </summary>
        public float AnimationSpeed { get; set; }

        private readonly AbcSimulationComponent moreDirty;
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationComponent"/> class.
        /// </summary>
        public AnimationComponent()
        {
            Sendable = true;
            moreDirty = TypeContainer.Get<AbcSimulationComponent>();
        }

        private float NextSmallerValue(float value)
        {
            if (value < 0)
                return BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(value) + 1);
            else if (value > 0)
                return BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(value) - 1);
            return -float.Epsilon;
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="model">The model to apply the animation on.</param>
        public void Update(GameTime gameTime, Model model)
        {
            if (model.CurrentAnimation is null)
                return;

            var oldTime = CurrentTime;

            CurrentTime = Math.Clamp(CurrentTime + AnimationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, NextSmallerValue(MaxTime));
            if (CurrentTime != oldTime)
                moreDirty.Add(this);

            model.UpdateAnimation(CurrentTime);
        }
    }
}
