using OctoAwesome.Pooling;

namespace OctoAwesome.Notifications
{

    public abstract class Notification : IPoolElement
    {

        public uint SenderId { get; set; }

        private IPool pool;

        public virtual bool Match<T>(T filter)
        {
            return true;
        }
        public void Init(IPool pool)
        {
            this.pool = pool;
            OnInit();
        }
        public void Release()
        {
            SenderId = 0;
            OnRelease();
            pool.Return(this);
        }

        /// <summary>
        /// This method is called from the Init method. It's not needed to hold an seperate pool
        /// </summary>
        protected virtual void OnInit()
        {

        }

        /// <summary>
        /// This is called from the Release method. Do not push this instance manualy to any pool 
        /// </summary>
        protected virtual void OnRelease()
        {

        }
    }
}
