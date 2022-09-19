using engenious;
using engenious.Graphics;
using engenious.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SixLabors.ImageSharp;

namespace OctoAwesome.Client.UI.Components
{
    /// <summary>
    /// Game component for managing asset resources.
    /// </summary>
    public sealed class AssetComponent : DrawableGameComponent
    {
        private readonly BaseScreenComponent screenComponent;
        private readonly ISettings settings;

        /// <summary>
        /// File name for resource pack info.
        /// </summary>
        public const string INFOFILENAME = "packinfo.xml";

        /// <summary>
        /// Settings key name for saving the currently active resource packs.
        /// </summary>
        public const string SETTINGSKEY = "ActiveResourcePacks";

        /// <summary>
        /// The path to search through for resource packs.
        /// </summary>
        public const string RESOURCEPATH = "Resources";
        readonly Dictionary<string, Texture2D> textures;
        readonly Dictionary<string, Image> bitmaps;
        readonly string[] textureTypes = new string[] { "png", "jpg", "jpeg", "bmp" };
        readonly List<ResourcePack> loadedPacks = new();
        readonly List<ResourcePack> activePacks = new();

        /// <summary>
        /// Gets a value indicating whether the asset manager is ready to load resources.
        /// </summary>
        public bool Ready { get; private set; }

        /// <summary>
        /// Gets the number of loaded textures.
        /// </summary>
        public int LoadedTextures => textures.Count + bitmaps.Count;

        /// <summary>
        /// Gets an enumeration of all available resource packs.
        /// </summary>
        public IEnumerable<ResourcePack> LoadedResourcePacks => loadedPacks.AsEnumerable();

        /// <summary>
        /// Gets an enumeration of all active resource packs.
        /// </summary>
        public IEnumerable<ResourcePack> ActiveResourcePacks => activePacks.AsEnumerable();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetComponent"/> class.
        /// </summary>
        /// <param name="screenComponent">The screen component used for asset loading.</param>
        /// <param name="settings">The settings i.a. used for asset handling.</param>
        public AssetComponent(BaseScreenComponent screenComponent, ISettings settings) : base(screenComponent.Game)
        {
            this.screenComponent = screenComponent;
            this.settings = settings;

            Ready = false;
            textures = new Dictionary<string, Texture2D>();
            bitmaps = new Dictionary<string, Image>();
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
                        if (resourcePack != null)
                            toLoad.Add(resourcePack);
                    }
                }
            }

            ApplyResourcePacks(toLoad);
        }

        /// <summary>
        /// Scan for resource packs in <see cref="RESOURCEPATH"/> directory.
        /// </summary>
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

        /// <summary>
        /// Applies a given enumeration of resource packs to be activated.
        /// </summary>
        /// <param name="packs">The resource packs to apply.</param>
        public void ApplyResourcePacks(IEnumerable<ResourcePack> packs)
        {
            Ready = false;

            // Reset already loaded assets
            foreach (var component in Game.Components.OfType<IAssetRelatedComponent>())
                component.UnloadAssets();

            // Dispose bitmaps
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

            // Set new active resource packs
            activePacks.Clear();
            foreach (var pack in packs)
                if (loadedPacks.Contains(pack))
                    activePacks.Add(pack);

            // Reload all resource pack dependant components.
            foreach (var component in Game.Components.OfType<IAssetRelatedComponent>())
                component.ReloadAssets();

            // Save active resource pack to the settings.
            settings.Set(SETTINGSKEY, string.Join(";", activePacks.Select(p => p.Path)));

            Ready = true;
        }

        /// <summary>
        /// Load a texture from the resource pack by a given key.
        /// </summary>
        /// <param name="key">The key name to load the texture for.</param>
        /// <returns>The loaded texture from the resource pack.</returns>
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

        /// <summary>
        /// Load a texture from the resource pack by a given key using a specific base type.
        /// </summary>
        /// <param name="baseType">The base type for the assembly to load the resource from.</param>
        /// <param name="key">The key name to load the texture for.</param>
        /// <returns>The loaded texture from the resource pack.</returns>
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

        /// <summary>
        /// Load a texture as a bitmap from the resource pack by a given key using a specific base type.
        /// </summary>
        /// <param name="baseType">The base type for the assembly to load the resource from.</param>
        /// <param name="key">The key name to load the texture for.</param>
        /// <returns>The loaded bitmap from the resource pack.</returns>
        public Image LoadBitmap(Type baseType, string key)
        {
            Image bitmap = default;

            if (screenComponent.GraphicsDevice.UiThread.IsOnGraphicsThread())
                return Load<Image>(baseType, key, textureTypes, bitmaps, (stream) => Image.Load(stream));

            screenComponent.Invoke(() =>
            {
                bitmap = Load(baseType, key, textureTypes, bitmaps, (stream) => Image.Load(stream));
            });

            return bitmap;
        }

        /// <summary>
        /// Load a stream from the resource pack by a given key using a specific base type.
        /// </summary>
        /// <param name="baseType">The base type for the assembly to load the resource from.</param>
        /// <param name="key">The key name to load the texture for.</param>
        /// <param name="fileTypes">The valid file types to load from disk as a fallback option.</param>
        /// <returns>The loaded bitmap from the resource pack.</returns>
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

            // Cache request
            var result = default(T);

            lock (textures)
                if (cache != null && cache.TryGetValue(fullkey, out result))
                    return result;

            // Try to load files for resource packs
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

            // Resource fallback
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

            // Load from OctoAwesome.Client.UI
            if (result is null)
                result = LoadFrom(typeof(ResourcePack), key, fileTypes, callback);

            if (result == null)
                // In worst case scenario: load the fallback checker-board texture
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OctoAwesome.Client.Assets.FallbackTexture.png"))
                    result = callback(stream);
            lock (textures)

                // Save to cache
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
