using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Tests
{
    public static class AssertEx
    {
        public static void AreEqual(Vector3 p1, Vector3 p2)
        {
            Assert.AreEqual(p1.X, p2.X, 0.0001f);
            Assert.AreEqual(p1.Y, p2.Y, 0.0001f);
            Assert.AreEqual(p1.Z, p2.Z, 0.0001f);
        }
    }
}