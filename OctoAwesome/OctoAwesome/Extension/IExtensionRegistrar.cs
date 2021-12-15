namespace OctoAwesome
{
    /// <summary>
    /// Interface for the generic Extension Loader.
    /// </summary>
    public interface IExtensionRegistrar<T> : IExtensionLoader
    {

        void Register(T value);

        void Unregister(T value);

    }
}
