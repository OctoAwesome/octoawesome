using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    public interface IInputSet
    {
        float MoveX { get; }

        float MoveY { get; }

        float HeadX { get; }

        float HeadY { get; }

        bool Interact { get; }
    }
}
