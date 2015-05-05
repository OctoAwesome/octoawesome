using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    internal class UpdateDomain
    {
        private IUniverse universe;

        public List<ActorHost> ActorHosts { get; private set; }

        public UpdateDomain()
        {
            ActorHosts = new List<ActorHost>();
            var host = new ActorHost(new Player());
            ActorHosts.Add(host);
        }

        public void Update(GameTime frameTime)
        {
            foreach (var actorHost in ActorHosts)
            {
                actorHost.Update(frameTime);
            }
        }



        public void Save()
        {
            ResourceManager.Instance.Save();
        }
    }
}
