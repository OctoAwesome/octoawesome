using System;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class EntityComponent : Component
    {
        /// <summary>
        /// Constructor of <see cref="EntityComponent"/>
        /// </summary>
        public EntityComponent()
        {
        }
        // TODO: validiate -> braucht man das wirklich ? :D
        //public void SetEntity(Entity entity)
        //{
        //    if (Entity != null)
        //        throw new NotSupportedException("Can not change the Entity");

        //    Entity = entity;
        //    OnSetEntity();
        //}
        //protected virtual void OnSetEntity()
        //{

        //}
    }
}
