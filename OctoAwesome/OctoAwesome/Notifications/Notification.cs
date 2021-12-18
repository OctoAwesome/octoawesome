using OctoAwesome.Pooling;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Base class for notifications.
    /// </summary>
    public abstract class Notification : IPoolElement
    {
        /// <summary>
        /// Gets or sets the id of the sender of this notification.
        /// </summary>
        public uint SenderId { get; set; }

        private IPool pool;

        /// <summary>
        /// Match this notification to a specific filter.
        /// </summary>
        /// <param name="filter">The value to filter by.</param>
        /// <typeparam name="T">The type to filter by.</typeparam>
        /// <returns>A value indicating whether this notification matches the given filter.</returns>
        public virtual bool Match<T>(T filter)
        {
            return true;
        }

        /// <inheritdoc />
        public void Init(IPool pool)
        {
            this.pool = pool;
            OnInit();
        }

        /// <inheritdoc />
        public void Release()
        {
            SenderId = 0;
            OnRelease();
            pool.Return(this);
        }

        /// <summary>
        /// This method is called from the Init method. It's not needed to hold a separate pool
        /// </summary>
        protected virtual void OnInit()
        {

        }

        /// <summary>
        /// This is called from the Release method. Do not push this instance manually to any pool-
        /// </summary>
        protected virtual void OnRelease()
        {

        }
    }
}
