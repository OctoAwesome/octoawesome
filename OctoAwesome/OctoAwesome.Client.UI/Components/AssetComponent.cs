using engenious;
using engenious.Graphics;
using engenious.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace OctoAwesome.Client.UI.Components
{

    public sealed class AssetComponent : DrawableGameComponent
    {
        private readonly BaseScreenComponent screenComponent;
        private readonly ISettings settings;
        public const string INFOFILENAME = "packinfo.xml";
        public const string SETTINGSKEY = "ActiveResourcePacks";
        public const string RESOURCEPATH = "Resources";
        readonly Dictionary<string, Texture2D> textures;
        readonly Dictionary<string, Bitmap> bitmaps;
        readonly string[] textureTypes = new string[] { "png", "jpg", "jpeg", "bmp" };
        readonly List<ResourcePack> loadedPacks = new();
        readonly List<ResourcePack> activePacks = new();

        /// <summary>
        /// Gibt an, ob der Asset Manager bereit zum Laden von Resourcen ist.
        /// </summary>
        public bool Ready { get; private set; }

        /// <summary>
        /// Gibt die Anzahl geladener Texturen zurück.
        /// </summary>
        public int LoadedTextures => textures.Count + bitmaps.Count;

        /// <summary>
        /// Auflistung aller bekannten Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> LoadedResourcePacks => loadedPacks.AsEnumerable();

        /// <summary>
        /// Auflistung aller aktuell aktiven Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> ActiveResourcePacks => activePacks.AsEnumerable();
        public AssetComponent(BaseScreenComponent screenComponent, ISettings settings) : base(screenComponent.Game)
        {
            this.screenComponent = screenComponent;
            this.settings = settings;

            Ready = false;
            textures = new Dictionary<string, Texture2D>();
            bitmaps = new Dictionary<string, Bitmap>();
            ScanForResourcePacks();

            // Load list of active Resource Packs
            var toLoad = new List<ResourcePack>();
            if (settings.KeyExists(SETTINGSKEY))
            {
                var activePackPathes = settings.Get<string>(SETTINGSKEY);
                if (!string.IsNullOrEmpty(activePackPathes))
                {
                    var packPathes = activePackPathes.Split(';');
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
                foreach (var directory in Directory.GetDirectories(RESOURCEPATH))
                {
                    var info = new DirectoryInfo(directory);
                    if (File.Exists(Path.Combine(directory, INFOFILENAME)))
                    {
                        // Scan info File
                        var serializer = new XmlSerializer(typeof(ResourcePack));
                        using Stream stream = File.OpenRead(Path.Combine(directory, INFOFILENAME));
                        var pack = (ResourcePack)serializer.Deserialize(stream);
                        pack!.Path = info.FullName;
                        loadedPacks.Add(pack);
                    }
                    else
                    {
                        var pack = new ResourcePack()
                        {
                            Path = info.FullName,
                            Name = info.Name
                        };

                        loadedPacks.Add(pack);
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
            settings.Set(SETTINGSKEY, string.Join(";", activePacks.Select(p => p.Path)));

            Ready = true;
        }
        public Texture2D LoadTexture(string key)
        {
            Texture2D texture2D = default;

            if (screenComponent.GraphicsDevice.UiThread.IsOnGraphicsThread())
                return Load(typeof(AssetComponent), key, textureTypes, textures, (stream) => Texture2D.FromStream(GraphicsDevice, stream));

            screenComponent.Invoke(() =>
            {
                texture2D = Load(typeof(AssetComponent), key, textureTypes, textures, (stream) => Texture2D.FromStream(GraphicsDevice, stream));
            });

            return texture2D;
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            Texture2D texture2D = default;

            if (screenComponent.GraphicsDevice.UiThread.IsOnGraphicsThread())
                return Load(baseType, key, textureTypes, textures, (stream) => Texture2D.FromStream(GraphicsDevice, stream));

            screenComponent.Invoke(() =>
            {
                texture2D = Load(baseType, key, textureTypes, textures, (stream) => Texture2D.FromStream(GraphicsDevice, stream));
            });

            return texture2D;

        }

        public Bitmap LoadBitmap(Type baseType, string key)
        {
            Bitmap bitmap = default;

            if (screenComponent.GraphicsDevice.UiThread.IsOnGraphicsThread())
                return Load(baseType, key, textureTypes, bitmaps, (stream) => (Bitmap)Image.FromStream(stream));

            screenComponent.Invoke(() =>
            {
                bitmap = Load(baseType, key, textureTypes, bitmaps, (stream) => (Bitmap)Image.FromStream(stream));
            });

            return bitmap;
        }
        public Stream LoadStream(Type baseType, string key, params string[] fileTypes)
        {
            return Load(baseType, key, fileTypes, null, (stream) =>
            {
                var result = new MemoryStream();
                var buffer = new byte[1024];
                int count;
                do
                {
                    count = stream.Read(buffer, 0, buffer.Length);
                    result.Write(buffer, 0, count);
                } while (count != 0);
                result.Seek(0, SeekOrigin.Begin);
                return result;
            });
        }

        private T Load<T>(Type baseType, string key, string[] fileTypes, Dictionary<string, T> cache, Func<Stream, T> callback)
        {
            if (baseType == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(key))
                return default;

            var fullkey = $"{baseType.Namespace}.{key}";

            var basefolder = baseType.Namespace!.Replace('.', Path.DirectorySeparatorChar);

            // Cache fragen
            var result = default(T);

            lock (textures)
                if (cache != null && cache.TryGetValue(fullkey, out result))
                    return result;

            // Versuche Datei zu laden
            foreach (var resourcePack in activePacks)
            {
                var localFolder = Path.Combine(resourcePack.Path, basefolder);

                foreach (var fileType in fileTypes)
                {
                    var filename = Path.Combine(localFolder, $"{key}.{fileType}");
                    if (File.Exists(filename))
                    {
                        using (var stream = File.Open(filename, FileMode.Open))
                            result = callback(stream);
                        break;
                    }
                }

                if (result != null)
                    break;
            }

            // Resource Fallback
            if (result is null)
            {
                result = LoadFrom(baseType, key, fileTypes, callback, assemblyName =>
                {
                    return assemblyName switch
                    {
                        "OctoClient" => "OctoAwesome.Client",
                        _ => assemblyName,
                    };
                });
            }

            // Aus OctoAwesome.Client.UI laden
            if (result is null)
                result = LoadFrom(typeof(ResourcePack), key, fileTypes, callback);

            if (result == null)
                // Im worstcase CheckerTex laden
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OctoAwesome.Client.Assets.FallbackTexture.png"))
                    result = callback(stream);
            lock (textures)

                // In Cache speichern
                if (result != null && cache != null)
                    cache[fullkey] = result;

            return result;
        }

        private TOut LoadFrom<TOut>(Type baseType, string ressourceKey, string[] fileExtensions, Func<Stream, TOut> callback, Func<string, string> replaceAssemblyName = null)
        {
            var fullkey = $"{baseType.Namespace}.{ressourceKey}";
            var assemblyName = baseType.Assembly.GetName().Name!;
            var resKey = fullkey.Replace(assemblyName, "");

            if (replaceAssemblyName is not null)
                assemblyName = replaceAssemblyName(assemblyName);

            resKey = $"{assemblyName}.Assets{resKey}";
            foreach (var fileExtension in fileExtensions)
            {
                using var stream = baseType.Assembly.GetManifestResourceStream($"{resKey}.{fileExtension}");
                if (stream is not null)
                    return callback(stream);
            }

            return default;
        }
    }
}
