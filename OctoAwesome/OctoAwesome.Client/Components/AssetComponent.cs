using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class AssetComponent : DrawableGameComponent
    {
        public const string INFOFILENAME = "packinfo.xml";

        public const string SETTINGSKEY = "ActiveResourcePacks";

        public const string RESOURCEPATH = "Resources";

        Dictionary<string, Texture2D> textures;

        Dictionary<string, Bitmap> bitmaps;

        string[] textureTypes = new string[] { "png", "jpg", "jpeg", "bmp" };

        List<ResourcePack> loadedPacks = new List<ResourcePack>();

        List<ResourcePack> activePacks = new List<ResourcePack>();

        /// <summary>
        /// Gibt an, ob der Asset Manager bereit zum Laden von Resourcen ist.
        /// </summary>
        public bool Ready { get; private set; }

        /// <summary>
        /// Gibt die Anzahl geladener Texturen zurück.
        /// </summary>
        public int LoadedTextures { get { return textures.Count + bitmaps.Count; } }

        /// <summary>
        /// Auflistung aller bekannten Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> LoadedResourcePacks { get { return loadedPacks.AsEnumerable(); } }

        /// <summary>
        /// Auflistung aller aktuell aktiven Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> ActiveResourcePacks { get { return activePacks.AsEnumerable(); } }

        public AssetComponent(OctoGame game) : base(game)
        {
            Ready = false;
            textures = new Dictionary<string, Texture2D>();
            bitmaps = new Dictionary<string, Bitmap>();
            ScanForResourcePacks();

            // Load list of active Resource Packs
            List<ResourcePack> toLoad = new List<ResourcePack>();
            if (SettingsManager.KeyExists(SETTINGSKEY))
            {
                string activePackPathes = SettingsManager.Get(SETTINGSKEY);
                if (!string.IsNullOrEmpty(activePackPathes))
                {
                    string[] packPathes = activePackPathes.Split(';');
                    foreach (var packPath in packPathes)
                    {
                        ResourcePack resourcePack = loadedPacks.FirstOrDefault(p => p.Path.Equals(packPath));
                        if (resourcePack != null) toLoad.Add(resourcePack);
                    }
                }
            }

            ApplyResourcePacks(toLoad);
        }

        public void ScanForResourcePacks()
        {
            loadedPacks.Clear();
            if (Directory.Exists(RESOURCEPATH))
            {
                foreach (var directory in Directory.GetDirectories(RESOURCEPATH))
                {
                    DirectoryInfo info = new DirectoryInfo(directory);
                    if (File.Exists(Path.Combine(directory, INFOFILENAME)))
                    {
                        // TODO: Scan info File
                    }
                    else
                    {
                        ResourcePack pack = new ResourcePack()
                        {
                            Path = info.FullName,
                            Name = info.Name
                        };

                        loadedPacks.Add(pack);
                    }
                }
            }
        }

        public void ApplyResourcePacks(IEnumerable<ResourcePack> packs)
        {
            Ready = false;

            // Reset vorhandener Assets
            foreach (var component in Game.Components.OfType<IAssetRelatedComponent>())
                component.UnloadAssets();

            // Dispose Bitmaps
            lock (bitmaps)
            {
                foreach (var value in bitmaps.Values)
                    value.Dispose();
                bitmaps.Clear();
            }

            // Dispose textures
            lock (textures)
            {
                foreach (var value in textures.Values)
                    value.Dispose();
                textures.Clear();
            }

            // Set new Active Resource Packs
            activePacks.Clear();
            foreach (var pack in packs)
                if (loadedPacks.Contains(pack)) // Warum eigentlich keine eigenen Packs?
                    activePacks.Add(pack);

            // Signal zum Reload senden
            foreach (var component in Game.Components.OfType<IAssetRelatedComponent>())
                component.ReloadAssets();

            // Speichern der Settings
            SettingsManager.Set(SETTINGSKEY, string.Join(";", activePacks.Select(p => p.Path)));

            Ready = true;
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            lock (textures)
            {
                return Load(baseType, key, textures, (stream) => Texture2D.FromStream(GraphicsDevice, stream));
            }
        }

        public Bitmap LoadBitmap(Type baseType, string key)
        {
            lock (bitmaps)
            {
                return Load(baseType, key, bitmaps, (stream) => (Bitmap)Image.FromStream(stream));
            }
        }

        private T Load<T>(Type baseType, string key, Dictionary<string, T> cache, Func<Stream, T> callback)
        {
            if (baseType == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException();

            string fullkey = string.Format("{0}.{1}", baseType.Namespace, key);

            string basefolder = baseType.Namespace.
                // Replace("OctoAwesome.", "").
                Replace('.', Path.DirectorySeparatorChar);

            // Cache fragen
            T result = default(T);
            if (cache.TryGetValue(fullkey, out result))
                return result;

            // Versuche Datei zu laden
            foreach (var resourcePack in activePacks)
            {
                string localFolder = Path.Combine(resourcePack.Path, basefolder);

                foreach (var textureType in textureTypes)
                {
                    string filename = Path.Combine(localFolder, string.Format("{0}.{1}", key, textureType));
                    if (File.Exists(filename))
                    {
                        using (var stream = File.Open(filename, FileMode.Open))
                        {
                            result = callback(stream);
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

                // Spezialfall Client
                if (assemblyName.Equals("OctoClient"))
                    assemblyName = "OctoAwesome.Client";

                var resKey = fullkey.Replace(assemblyName, string.Format("{0}.Assets", assemblyName));
                foreach (var texturetype in textureTypes)
                {
                    using (var stream = baseType.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", resKey, texturetype)))
                    {
                        if (stream != null)
                        {
                            result = callback(stream);
                            break;
                        }
                    }
                }
            }

            if (result == null)
            {
                // Im worstcase CheckerTex laden
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OctoAwesome.Client.Assets.FallbackTexture.png"))
                {
                    result = callback(stream);
                }
            }

            // In Cache speichern
            if (result != null)
                cache[fullkey] = result;

            return result;
        }
    }
}
