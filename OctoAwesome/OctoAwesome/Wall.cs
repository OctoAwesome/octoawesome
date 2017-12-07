using System;

namespace OctoAwesome
{
    [Flags]
    public enum Wall
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        Back,
        Front
    }
}