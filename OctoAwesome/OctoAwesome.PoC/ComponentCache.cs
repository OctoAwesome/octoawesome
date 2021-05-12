using OctoAwesome.EntityComponents;

using System;

namespace OctoAwesome.PoC
{
    public class ComponentCache : Cache<int, Component>
    {
        protected override Component Load(int key)
        {
            throw new NotImplementedException();
        }
    }

    public class EntityCache : Cache<PositionComponent, Entity>
    {
        protected override Entity Load(PositionComponent key)
        {
            throw new NotImplementedException();
        }
    }
}
