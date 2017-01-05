using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.Entities
{
    public class WauziEntity : UpdateableEntity
    {
        public int JumpTime { get; set; }

        public WauziEntity() : base()
        {
        }

        protected override void OnInitialize(IResourceManager manager)
        {
            Cache = new LocalChunkCache(manager.GlobalChunkCache, true, 2, 1);
        }

        public override void Update(GameTime gameTime)
        {
            BodyPowerComponent body = Components.GetComponent<BodyPowerComponent>();
            ControllableComponent controller = Components.GetComponent<ControllableComponent>();
            controller.MoveInput = new Vector2(0.5f, 0.5f) ;

            if (JumpTime <= 0)
            {
                controller.JumpInput = true;
                JumpTime = 10000;
            }
            else
            {
                JumpTime -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (controller.JumpActive)
            {
                controller.JumpInput = false;
            }
        }
    }
}
