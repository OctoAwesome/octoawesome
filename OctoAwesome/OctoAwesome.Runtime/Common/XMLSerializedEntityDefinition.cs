using OctoAwesome.Entities;
using System;
using System.Collections.Generic;
namespace OctoAwesome.Runtime.Common
{
    class XMLSerializedEntityDefinition : IEntityDefinition
    {
        public Type EntityType { get; internal set; }

        // TODO: ist das sinnvoll oder legetim, da das IInventoryable interface implementiert wird?
        public bool IsInventoryable { get; internal set; }

        public string Name { get; internal set; }

        public float RotationZ { get; internal set; }

        public float Mass { get; internal set; }

        public float Radius { get; internal set; }

        public float Height { get; internal set; }

        public decimal VolumePerUnit { get; internal set; }

        public int StackLimit { get; internal set; }

        public string Icon { get; internal set; }

        //private readonly DependencyProperty<bool> dependencyisinvertoryable;
        //private readonly DependencyProperty<bool> dependencyentitytype;
        //private readonly DependencyProperty<bool> dependencyname;
        //private readonly DependencyProperty<bool> dependencyicon;
        //private readonly DependencyProperty<bool> dependencyvolumeperunit;
        //private readonly DependencyProperty<bool> dependencystacklimit;
        //private readonly DependencyProperty<bool> dependencyrotationz;
        //private readonly DependencyProperty<bool> dependencymass;
        //private readonly DependencyProperty<bool> dependencyradius;
        //private readonly DependencyProperty<bool> dependencyheight;
        private Dictionary<string, object> resources;
        public XMLSerializedEntityDefinition()
        {
            resources = new Dictionary<string, object>();
        }
        public bool TryGetResource<T>(string name, out T resource)
        {
            if (resources.TryGetValue(name, out object res) && res is T resou)
            {
                resource = resou;
                return true;
            }
            resource = default(T);
            return false;
        }
        internal void Add(string key, object obj)
        {
            if (!resources.ContainsKey(key))
                resources[key] = obj;
            else resources.Add(key, obj);
        }
    }
}
