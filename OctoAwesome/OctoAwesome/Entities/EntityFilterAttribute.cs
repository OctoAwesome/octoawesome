using engenious.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
