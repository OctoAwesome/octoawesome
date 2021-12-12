namespace OctoAwesome.Pooling
{

    public interface IPool
    {
        void Return(IPoolElement obj);
    }
    public interface IPool<T> : IPool where T : IPoolElement
    {

        T Rent();

        void Return(T obj);
    }
}
