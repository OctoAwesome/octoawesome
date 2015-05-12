using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Tests
{
    [TestClass]
    public class BlockIntersectTests
    {
        IBlock block;

        [TestInitialize]
        public void Init()
        {
            block = new TestBlock(new Index3(10, 20, 30));
        }

        [TestCleanup]
        public void Cleanup()
        {
            block = null;
        }

        /// <summary>
        /// Testet das Verhalten wenn der Playerblock zu weit Weg für eine Kollision ist
        /// </summary>
        [TestMethod]
        public void BlockIntersectFromOutside()
        {
            // X-Achse links zu weit weg (keine Bewegung)
            Axis? collisionAxis;
            float? distance = block.Intersect(new Vector3(5, 20, 30), new Vector3(), out collisionAxis);
            Assert.AreEqual(null, collisionAxis);
            Assert.AreEqual(null, distance);

            // X-Achse links zu weit weg (bewegung nach links)
            distance = block.Intersect(new Vector3(5, 20, 30), new Vector3(-4, 0, 0), out collisionAxis);
            Assert.AreEqual(null, collisionAxis);
            Assert.AreEqual(null, distance);

            // X-Achse links zu weit weg (bewegung nach rechts)
            distance = block.Intersect(new Vector3(5, 20, 30), new Vector3(4, 0, 0), out collisionAxis);
            Assert.AreEqual(null, collisionAxis);
            Assert.AreEqual(null, distance);
        }

        /// <summary>
        /// Testet das Handling bei vollständiger Überschneidung
        /// </summary>
        [TestMethod]
        public void BlockIntersectPreviousCollision()
        {
            // X-Achse bereits links überschnitten (keine Bewegung) 
            // !!! Keine Chance der Auflösung - einfach ignorieren ;)
            Axis? collisionAxis;
            float? distance = block.Intersect(new Vector3(10.5f, 20.5f, 30.5f), new Vector3(), out collisionAxis);
            Assert.AreEqual(null, collisionAxis);
            Assert.AreEqual(null, distance);

            // X-Achse bereits links überschnitten (Bewegung nach links)
            distance = block.Intersect(new Vector3(10.5f, 20.5f, 30.5f), new Vector3(-1, 0, 0), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(-0.5f, distance);

            // X-Achse bereits links überschnitten (Bewegung nach rechts)
            distance = block.Intersect(new Vector3(10.5f, 20.5f, 30.5f), new Vector3(1, 0, 0), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(-0.5f, distance);
        }

        /// <summary>
        /// Prüfung, ob die richtigen Achsen und Distanz erkannt werden
        /// </summary>
        [TestMethod]
        public void BlockIntersectAxisCheck()
        {
            // Kollisionsprüfung auf +X Achse
            Axis? collisionAxis;
            float? distance = block.Intersect(new Vector3(9, 20, 30), new Vector3(2, 0, 0), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf -X Achse
            distance = block.Intersect(new Vector3(12, 20, 30), new Vector3(-2, 0, 0), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf +Y Achse
            distance = block.Intersect(new Vector3(10, 19, 30), new Vector3(0, 2, 0), out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf -Y Achse
            distance = block.Intersect(new Vector3(10, 22, 30), new Vector3(0, -2, 0), out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf +Z Achse
            distance = block.Intersect(new Vector3(10, 20, 29), new Vector3(0, 0, 2), out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf -Z Achse
            distance = block.Intersect(new Vector3(10, 20, 32), new Vector3(0, 0, -2), out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);
        }

        /// <summary>
        /// Testet Achsenpriorisierung
        /// </summary>
        [TestMethod]
        public void BlockIntersectDistanceCheck()
        {
            // Bewegung von links oben mit Kollision auf X
            Axis? collisionAxis;
            float? distance = block.Intersect(new Vector3(9, 19, 29), new Vector3(2, 3, 3), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von links oben mit Kollision auf Y
            distance = block.Intersect(new Vector3(9, 19, 29), new Vector3(3, 2, 3), out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von links oben mit Kollision auf Z
            distance = block.Intersect(new Vector3(9, 19, 29), new Vector3(3, 3, 2), out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von rechts unten mit Kollision auf X
            distance = block.Intersect(new Vector3(12, 22, 32), new Vector3(-2, -3, -3), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von rechts unten mit Kollision auf Y
            distance = block.Intersect(new Vector3(12, 22, 32), new Vector3(-3, -2, -3), out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von rechts unten mit Kollision auf Z
            distance = block.Intersect(new Vector3(12, 22, 32), new Vector3(-3, -3, -2), out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);
        }

        /// <summary>
        /// Durchdrungene Achsen ignorieren
        /// </summary>
        [TestMethod]
        public void BlockInteractSliding()
        {
            Axis? collisionAxis;
            float? distance = block.Intersect(new Vector3(9, 20.5f, 30), new Vector3(2, 0.5f, 0), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);
        }

        /// <summary>
        /// Durchdringung abfangen
        /// </summary>
        public void BlockInteractDiffusion()
        {
            // Muss trotz vollständiger Durchdringung eine Kollision ermitteln
            Axis? collisionAxis;
            float? distance = block.Intersect(new Vector3(9, 19, 29), new Vector3(5, 5, 5), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(1f / 3f, distance);
        }
    }
}
