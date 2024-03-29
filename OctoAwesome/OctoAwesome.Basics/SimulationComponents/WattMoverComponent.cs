﻿using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using System;
using System.Diagnostics;
using engenious;
using engenious.Helper;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with watt energy system.
    /// </summary>
    public class WattMoverComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent>,
        ControllableComponent,
        BodyPowerComponent>
    {
        /// <inheritdoc />
        protected override SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent> OnAdd(Entity entity)
        {
            var controllable = entity.GetComponent<ControllableComponent>();
            var bodyPower = entity.GetComponent<BodyPowerComponent>();
            Debug.Assert(controllable != null, nameof(controllable) + " != null");
            Debug.Assert(bodyPower != null, nameof(bodyPower) + " != null");
            return new SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent>(entity, controllable, bodyPower);
        }

        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, BodyPowerComponent> value)
        {
            //Move

            var e = value.Value;
            var controller = value.Component1;
            var powercomp = value.Component2;

            if (e.Components.TryGet<HeadComponent>(out var head))
            {
                float lookX = (float)Math.Cos(head.Angle);
                float lookY = -(float)Math.Sin(head.Angle);
                var velocitydirection = new Vector3(lookX, lookY, 0) * controller.MoveInput.Y;

                float stafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
                float stafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);
                velocitydirection += new Vector3(stafeX, stafeY, 0) * controller.MoveInput.X;

                powercomp.Direction = velocitydirection;

            }
            else
            {
                powercomp.Direction = new Vector3(controller.MoveInput.X, controller.MoveInput.Y);
            }

            //Jump
            if (controller.JumpInput && !controller.JumpActive)
            {
                controller.JumpTime = powercomp.JumpTime;
                controller.JumpActive = true;
            }

            if (controller.JumpActive)
            {
                powercomp.Direction += new Vector3(0, 0, 1);
                controller.JumpTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (controller.JumpTime <= 0)
                    controller.JumpActive = false;
            }

        }
    }
}
