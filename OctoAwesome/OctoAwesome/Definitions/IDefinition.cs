using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Base Interface for all definitions.
    /// </summary>
    public interface IDefinition
    {
        /// <summary>
        /// Gets the name of the definition.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the name of the icon resource.
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Gets the Categories associated with this definition to group or filter by
        /// </summary>
        string[] Categories { get; }
    }
}
