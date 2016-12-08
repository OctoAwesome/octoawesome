using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public class ToolBarComponent : EntityComponent
    {
        /// <summary>
        /// Gibt die Anzahl Tools in der Toolbar an.
        /// </summary>
        public const int TOOLCOUNT = 10;

        /// <summary>
        /// Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
        /// </summary>
        public InventorySlot[] Tools { get; set; }
        public InventorySlot ActiveTool { get; set; }

        public ToolBarComponent()
        {
            Tools = new InventorySlot[TOOLCOUNT];
        }
    }
}
