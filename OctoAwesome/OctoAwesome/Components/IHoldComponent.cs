namespace OctoAwesome.Components
{
    public interface IHoldComponent<in T>
    {
        void Add(T value);

        void Remove(T value);
    }
}
