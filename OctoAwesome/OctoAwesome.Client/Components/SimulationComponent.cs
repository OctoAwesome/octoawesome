using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        public World World { get; private set; }

        public ActorHost Player { get; private set; }
        
        public SimulationComponent(Game game) : base(game) { }

        public override void Initialize()
        {
            World = new World();

            var p = Load();
            Player = World.InjectPlayer(p);
            Player.Initialize();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }

        internal void Save()
        {
            Save(Player.Player);
        }

        public void Save(Player player)
        {
            var root = GetRoot();

            string filename = "player.info";
            using (Stream stream = File.Open(root.FullName + Path.DirectorySeparatorChar + filename, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Player));
                serializer.Serialize(stream, player);
            }
        }

        public Player Load()
        {
            var root = GetRoot();
            string filename = "player.info";

            if (!File.Exists(root.FullName + Path.DirectorySeparatorChar + filename))
                return new Player();

            using (Stream stream = File.Open(root.FullName + Path.DirectorySeparatorChar + filename, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Player));
                    return (Player)serializer.Deserialize(stream);
                }
                catch (Exception) { }

                return new Player();
            }
        }

        private DirectoryInfo GetRoot()
        {
            string appconfig = ConfigurationManager.AppSettings["ChunkRoot"];
            if (!string.IsNullOrEmpty(appconfig))
            {
                return new DirectoryInfo(appconfig);
            }
            else
            {
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                DirectoryInfo root = new DirectoryInfo(exePath + Path.DirectorySeparatorChar + "OctoMap");
                if (!root.Exists) root.Create();
                return root;
            }
        }
    }
}
