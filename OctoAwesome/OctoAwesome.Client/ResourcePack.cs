﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OctoAwesome.Client
{
    public sealed class ResourcePack
    {
        [XmlIgnore]
        public string Path { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public string Icon { get; set; }
    }
}
