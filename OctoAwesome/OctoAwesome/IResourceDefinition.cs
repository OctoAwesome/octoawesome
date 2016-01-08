using System;
using System.Drawing;

namespace OctoAwesome
{
    public interface IResourceDefinition : IItemDefinition
    {
        IResource GetInstance();

        Type GetResourceType();
    }
}