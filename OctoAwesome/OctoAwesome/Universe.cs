using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class Universe : IUniverse
    {
        private Dictionary<int, IPlanet> planets;

        public Universe(int id, string name)
        {
            Id = id;
            Name = name;
            planets = new Dictionary<int, IPlanet>();
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public IPlanet GetPlanet(int id)
        {
            if (planets.ContainsKey(id))
                return planets[id];
            return null;
        }

        public void SetPlanet(IPlanet planet)
        {
            planets.Add(planet.Id, planet);
        }
    }
}
