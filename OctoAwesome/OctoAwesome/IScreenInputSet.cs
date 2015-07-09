using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IScreenInputSet
    {
        Index2 PointerPosition { get; set; }
        
        event OnKeyChange OnKeyDown;

        event OnKeyChange OnKeyUp;
    }

    public delegate void OnKeyChange(Keys key);
}
