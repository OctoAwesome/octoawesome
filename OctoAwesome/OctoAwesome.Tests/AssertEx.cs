using System;
using engenious;
using NUnit.Framework;

namespace OctoAwesome.Tests
{
    public static class AssertEx
    {
        public static void Equals(Vector3 p1, Vector3 p2)
        {
            var diff = p1 - p2;
            if (Math.Abs(p1.X) < 1f)
                Assert.Less(Math.Abs(diff.X), 0.00001f);
            else
                Assert.That(p1.X, Is.EqualTo(p2.X).Within(4).Ulps);

            if (Math.Abs(p1.Y) < 1f)
                Assert.Less(Math.Abs(diff.Y), 0.00001f);
            else
                Assert.That(p1.Y, Is.EqualTo(p2.Y).Within(4).Ulps);

            if (Math.Abs(p1.Z) < 1f)
                Assert.Less(Math.Abs(diff.Z), 0.00001f);
            else
                Assert.That(p1.Z, Is.EqualTo(p2.Z).Within(4).Ulps);
        }
    }
}