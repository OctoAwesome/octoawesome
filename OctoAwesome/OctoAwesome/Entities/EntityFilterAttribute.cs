using System;
namespace OctoAwesome.Entities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class EntityFilterAttribute : Attribute
    {
        public Type[] EntityComponentTypes { get; set; }

        public EntityFilterAttribute(params Type[] entityComponentTypes) 
            => EntityComponentTypes = entityComponentTypes;
    }
}
