using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class AssetComponent : DrawableGameComponent
    {
        Dictionary<string, Texture2D> cache;

        List<string> resourcePacks = new List<string>();

        public AssetComponent(OctoGame game) : base(game)
        {
            cache = new Dictionary<string, Texture2D>();
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            throw new NotImplementedException();

            string folder = baseType.Namespace;
            // folder/key.png

            if (cache.ContainsKey(folder))
                return cache[folder];

            // TODO: Load Resources from Resource Packs

            // TODO: Fallback: Load Resources from Mod-Assembly Resource.resx
        }
    }
}
