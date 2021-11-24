using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.UI.Components
{
    /// <summary>
    /// Component to provide a ui for the screen
    /// </summary>
    public abstract class UIComponent : Component
    {
        public bool Visible { get; set; }
        /// <summary>
        /// Displays the ui
        /// </summary>
        public abstract void Draw(GameTime gameTime);
    }
}
