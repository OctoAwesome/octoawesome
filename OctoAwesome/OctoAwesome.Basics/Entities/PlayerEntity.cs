using OctoAwesome.Basics.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Entities
{
    public class PlayerEntity : Entity
    {
        public PlayerEntity()
        {
            Components.AddComponent(new GravityComponent());
            Components.AddComponent(new BodyComponent() { Mass = 50f });
            Components.AddComponent(new BodyPowerComponent() { Power = 600f });
            Components.AddComponent(new JumpPowerComponent() { Power = 400000f });
            Components.AddComponent(new MoveableComponent());
            Components.AddComponent(new BoxCollisionComponent());
            Components.AddComponent(new EntityCollisionComponent());
        }
    }
}
