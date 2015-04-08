using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    internal sealed class DebugData
    {
        private static DebugData instance;

        public  static DebugData Instance
        {
            get
            {
                if (instance == null)
                    instance = new DebugData();
                return instance;
            }
        }
        private DebugData()
        {
            LightEnabled = true;
            AmbientColor = Vector3.One * 0.8f;
            DiffuseColor = Vector3.One * 0.8f;
            EmissiveColor = Vector3.Zero;

            DirectionalDirection = -Vector3.One;
            DirectionalColor = new Vector3(0.8f, 1f, 1f);
        }

        public bool LightEnabled { get; set; }

        public Vector3 AmbientColor { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 EmissiveColor { get; set; }

        public Vector3 DirectionalColor { get; set; }

        public Vector3 DirectionalDirection { get; set; }

    }
}
