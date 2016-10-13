using OctoAwesome.Basics.Components;

namespace OctoAwesome.Basics.Entities
{
    public class AppleEntity : Entity
    {
        public AppleEntity()
        {
            AddComponent(new CollidableComponent() { });
        }
    }
}
