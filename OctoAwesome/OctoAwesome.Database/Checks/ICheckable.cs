namespace OctoAwesome.Database.Checks
{
    /// <summary>
    /// Interface for instances that can be checked.
    /// </summary>
    public interface ICheckable
    {
        /// <summary>
        /// Checks this instance.
        /// </summary>
        /// <exception cref="System.Exception">Thrown when the check failed.</exception>
        void Check();
    }
}