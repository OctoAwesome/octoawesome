using OctoAwesome.Components;
using System.IO;
using System.Runtime.CompilerServices;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all components.
    /// </summary>
    public abstract class Component : IComponent
    {
        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public bool Sendable { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        protected Component()
        {
            Enabled = true;
            Sendable = false;
        }

        /// <inheritdoc />
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Enabled);
        }

        /// <inheritdoc />
        public virtual void Deserialize(BinaryReader reader)
        {
            Enabled = reader.ReadBoolean();
        }

        /// <summary>
        /// Called when a value is changed.
        /// </summary>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property that was changed.</param>
        /// <typeparam name="T">The type of the property that was changed.</typeparam>
        protected virtual void OnPropertyChanged<T>(T value, string propertyName)
        {

        }

        /// <summary>
        /// Sets a field to a new value and propagates property changes.
        /// </summary>
        /// <param name="field">The reference to the field whose value to change.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The name of the property to set the value of.</param>
        /// <typeparam name="T">The type of the property to set the value of.</typeparam>
        protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (field != null)
            {
                if (field.Equals(value))
                    return;
            }

            field = value;

            OnPropertyChanged(field, propertyName);
        }
    }
}
