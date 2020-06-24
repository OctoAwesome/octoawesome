using engenious;
using NUnit.Framework;

namespace OctoAwesome.Tests
{
    public static class AssertEx
    {
        public static void Equals(Vector3 p1, Vector3 p2)
        {
            Assert.That(p1.X, Is.EqualTo(p2.X).Within(4).Ulps);
            Assert.That(p1.Y, Is.EqualTo(p2.Y).Within(4).Ulps);
            Assert.That(p1.Z, Is.EqualTo(p2.Z).Within(4).Ulps);
        }
    }
}