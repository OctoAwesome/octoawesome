using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class AssetComponent : DrawableGameComponent
    {
        Dictionary<string, Texture2D> cache;

        string[] textureTypes = new string[] { "png", "jpg", "jpeg", "bmp" };

        List<string> resourcePacks = new List<string>();

        /// <summary>
        /// Gibt die Anzahl geladener Texturen zurück.
        /// </summary>
        public int LoadedTextures { get { return cache.Count; } }

        public AssetComponent(OctoGame game) : base(game)
        {
            cache = new Dictionary<string, Texture2D>();
            resourcePacks.Add("PetersTexturePack");
            resourcePacks.Add("Toms Tolle Texturen");
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            if (baseType == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException();

            string fullkey = string.Format("{0}.{1}", baseType.Namespace, key);

            // OctoAwesome.Basics.Assets.Definitions.blocks.ice

            string basefolder = baseType.Namespace.
                // Replace("OctoAwesome.", "").
                Replace('.', Path.DirectorySeparatorChar);

            // Cache fragen
            Texture2D result = null;
            if (cache.TryGetValue(fullkey, out result))
                return result;

            // Versuche Datei zu laden
            foreach (var resourcePack in resourcePacks)
            {
                string localFolder = Path.Combine("Resources", resourcePack, basefolder);

                foreach (var textureType in textureTypes)
                {
                    string filename = Path.Combine(localFolder, string.Format("{0}.{1}", key, textureType));
                    if (File.Exists(filename))
                    {
                        using (var stream = File.Open(filename, FileMode.Open))
                        {
                            result = Texture2D.FromStream(GraphicsDevice, stream);
                        }
                        break;
                    }
                }

                if (result != null)
                    break;
            }

            // Resource Fallback
            if (result == null)
            {
                var assemblyName = baseType.Assembly.GetName().Name;
                var resKey = fullkey.Replace(assemblyName, string.Format("{0}.Assets", assemblyName));
                foreach (var texturetype in textureTypes)
                {
                    using (var stream = baseType.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", resKey, texturetype)))
                    {
                        if (stream != null)
                        {
                            result = Texture2D.FromStream(GraphicsDevice, stream);
                            break;
                        }
                    }
                }
            }

            if (result == null)
            {
                // Im worstcase CheckerTex laden
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OctoAwesome.Client.Assets.OctoAwesome.Client.FallbackTexture.png"))
                {
                    result = Texture2D.FromStream(GraphicsDevice, stream);
                }
            }

            // In Cache speichern
            if (result != null)
                cache[fullkey] = result;

            return result;
        }
    }
}
