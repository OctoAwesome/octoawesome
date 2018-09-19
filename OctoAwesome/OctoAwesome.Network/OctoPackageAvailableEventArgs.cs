using System;

namespace OctoAwesome.Network
{
    public class OctoPackageAvailableEventArgs : EventArgs
    {
        public BaseClient BaseClient { get; set; }
        public Package Package { get; set; }
    }
}
