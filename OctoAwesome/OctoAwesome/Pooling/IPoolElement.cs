namespace OctoAwesome.Pooling
{
    public interface IPoolElement
    {
        void Init(IPool pool);
        void Release();
    }
}