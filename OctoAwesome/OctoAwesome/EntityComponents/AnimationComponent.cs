using engenious;
using engenious.Graphics;

using OctoAwesome.Components;

using System;
using System.IO;

namespace OctoAwesome.EntityComponents
{

    public class AnimationComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {

        public float CurrentTime { get; set; }
        public float MaxTime { get; set; }
        public float AnimationSpeed { get; set; }
        public AnimationComponent()
        {
            Sendable = true;
        }
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(CurrentTime);
            writer.Write(MaxTime);
            writer.Write(AnimationSpeed);
            base.Serialize(writer);
        }
        public override void Deserialize(BinaryReader reader)
        {
            CurrentTime = reader.ReadSingle();
            MaxTime = reader.ReadSingle();
            AnimationSpeed = reader.ReadSingle();
            base.Deserialize(reader);
        }

        private float NextSmallerValue(float value)
        {
            if (value < 0)
                return BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(value) + 1);
            else if (value > 0)
                return BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(value) - 1);
            return -float.Epsilon;
        }
        public void Update(GameTime gameTime, Model model)
        {
            if (model.CurrentAnimation is null)
                return;

            CurrentTime = Math.Clamp(CurrentTime + AnimationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, NextSmallerValue(MaxTime));

            model.UpdateAnimation(CurrentTime);
        }
    }
}
