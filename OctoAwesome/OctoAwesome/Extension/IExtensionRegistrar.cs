namespace OctoAwesome
{
    /// <summary>
    /// Interface for the generic Extension Loader.
    /// </summary>
    public interface IExtensionRegistrar<TRegister, TUnregister> : IExtensionLoader
    {

        void Register(TRegister value);

        void Unregister(TUnregister value);

    }
}
