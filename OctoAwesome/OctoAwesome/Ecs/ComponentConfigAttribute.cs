using System;

namespace OctoAwesome.Ecs
{
    public class ComponentConfigAttribute : Attribute
    {
        public int Prefill;

        public ComponentConfigAttribute(int prefill = 16)
        {
            Prefill = prefill;
        }
    }
}