namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Components.
    /// </summary>
    public abstract class Component
    {
        public bool Enabled { get; set; }

        public Component()
        {
            Enabled = true;
        }
    }
}
