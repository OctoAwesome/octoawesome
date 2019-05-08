using engenious;
using Xunit;

namespace OctoAwesome.Tests
{
    public static class AssertEx
    {
        public static void Equal(Vector3 p1, Vector3 p2)
        {
            Assert.Equal(p1.X, p2.X, 4);
            Assert.Equal(p1.Y, p2.Y, 4);
            Assert.Equal(p1.Z, p2.Z, 4);
        }
    }
}