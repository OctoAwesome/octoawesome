using OctoAwesome.Notifications;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class EntityComponent : Component
    {
        /// <summary>
        /// Reference to the Entity.
        /// </summary>
        public Entity Entity { get; private set; }
        
        public void SetEntity(Entity entity)
        {
            if (Entity != null)
                throw new NotSupportedException("Can not change the Entity");

            Entity = entity;
            OnSetEntity();
        }

        public virtual void OnUpdate(Notification notification)
        {

        }

        protected virtual void OnSetEntity()
        {

        }

        protected virtual void Update(Notification notification)
        {
            Entity?.OnUpdate(notification);
        }
        
    }
}
