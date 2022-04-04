namespace OctoAwesome.Components
{
    public interface IComponentContainer
    {
        bool ContainsComponent<T>();

        T? GetComponent<T>();
    }
}
