using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OctoAwesome.Tests
{
    [TestClass]
    public class BlockIntersectTests
    {
        BlockDefinition block = new TestBlockDefinition();

        [TestInitialize]
        public void Init()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        /// <summary>
        /// Testet das Verhalten wenn der Playerblock zu weit Weg für eine Kollision ist
        /// </summary>
        [TestMethod]
        public void BlockIntersectFromOutside()
        {
            BoundingBox player = new BoundingBox(new Vector3(5, 20, 30), new Vector3(6, 21, 31));

            // X-Achse links zu weit weg (keine Bewegung)
            Axis? collisionAxis;
            float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0),
                new Index3(10, 20, 30), player, new Vector3(), out collisionAxis);
            Assert.AreEqual(null, collisionAxis);
            Assert.AreEqual(null, distance);

            // X-Achse links zu weit weg (bewegung nach links)
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0),
                new Index3(10, 20, 30), player, new Vector3(-4, 0, 0), out collisionAxis);
            Assert.AreEqual(null, collisionAxis);
            Assert.AreEqual(null, distance);

            // X-Achse links zu weit weg (bewegung nach rechts)
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0),
                new Index3(10, 20, 30), player, new Vector3(4, 0, 0), out collisionAxis);
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
            float? distance = Block.Intersect(
                block.GetCollisionBoxes(null, 0, 0, 0),
                new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 20, 30), new Vector3(11, 21, 31)), new Vector3(), out collisionAxis);
            Assert.AreEqual(null, collisionAxis);
            Assert.AreEqual(null, distance);

            // X-Achse bereits links überschnitten (Bewegung nach links)
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 20, 30), new Vector3(11, 21, 31)), new Vector3(-2, 0, 0),
                out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(-0.5f, distance);

            // X-Achse bereits links überschnitten (Bewegung nach rechts)
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 20, 30), new Vector3(11, 21, 31)), new Vector3(2, 0, 0),
                out collisionAxis);
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
            float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(8, 20, 30), new Vector3(9, 21, 31)), new Vector3(2, 0, 0), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf -X Achse
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(12, 20, 30), new Vector3(13, 21, 31)), new Vector3(-2, 0, 0),
                out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf +Y Achse
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 18, 30), new Vector3(11, 19, 31)), new Vector3(0, 2, 0),
                out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf -Y Achse
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 22, 30), new Vector3(11, 23, 31)), new Vector3(0, -2, 0),
                out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf +Z Achse
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 20, 28), new Vector3(11, 21, 29)), new Vector3(0, 0, 2),
                out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Kollisionsprüfung auf -Z Achse
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 20, 32), new Vector3(11, 21, 33)), new Vector3(0, 0, -2),
                out collisionAxis);
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
            float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(8, 18, 28), new Vector3(9, 19, 29)), new Vector3(2, 3, 3), out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von links oben mit Kollision auf Y
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(8, 18, 28), new Vector3(9, 19, 29)), new Vector3(3, 2, 3), out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von links oben mit Kollision auf Z
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(8, 18, 28), new Vector3(9, 19, 29)), new Vector3(3, 3, 2), out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von rechts unten mit Kollision auf X
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(12, 22, 32), new Vector3(13, 23, 33)), new Vector3(-2, -3, -3),
                out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von rechts unten mit Kollision auf Y
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(12, 22, 32), new Vector3(13, 23, 33)), new Vector3(-3, -2, -3),
                out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // Bewegung von rechts unten mit Kollision auf Z
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(12, 22, 32), new Vector3(13, 23, 33)), new Vector3(-3, -3, -2),
                out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);
        }

        /// <summary>
        /// Durchdrungene Achsen ignorieren
        /// </summary>
        [TestMethod]
        public void BlockIntersectSliding()
        {
            // X
            Axis? collisionAxis;
            float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(8, 20.5f, 30), new Vector3(9, 21.5f, 31)), new Vector3(2, 0.5f, 0),
                out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);
        }

        /// <summary>
        /// Durchdringung abfangen
        /// </summary>
        public void BlockIntersectDiffusion()
        {
            // Muss trotz vollständiger Durchdringung eine Kollision ermitteln
            Axis? collisionAxis;
            float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(9, 19, 29), new Vector3(10, 20, 30)), new Vector3(5, 5, 5),
                out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(1f/3f, distance);
        }

        /// <summary>
        /// Testet die Kollision, wenn eine Ecke kollidiert, die nicht die Move-Ecke des Spielers ist.
        /// </summary>
        [TestMethod]
        public void OppositCornerIntersect()
        {
            // x
            Axis? collisionAxis;
            float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(8, 20, 30), new Vector3(9, 21, 31)), new Vector3(2, 1.5f, 0),
                out collisionAxis);
            Assert.AreEqual(Axis.X, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // y
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 18, 30), new Vector3(11, 19, 31)), new Vector3(0, 2f, 1.5f),
                out collisionAxis);
            Assert.AreEqual(Axis.Y, collisionAxis);
            Assert.AreEqual(0.5f, distance);

            // z
            distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(10, 20, 28), new Vector3(11, 21, 29)), new Vector3(0, 1.5f, 2),
                out collisionAxis);
            Assert.AreEqual(Axis.Z, collisionAxis);
            Assert.AreEqual(0.5f, distance);
        }

        /// <summary>
        /// Prüft das Verhalten der Kollision bei Blocks, die sich knapp übers Eck aneinander vorbei bewegen
        /// </summary>
        [TestMethod]
        public void NonContactCornerIntersect()
        {
            // x
            Axis? collisionAxis;
            float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30),
                new BoundingBox(new Vector3(8, 20, 30), new Vector3(9, 21, 31)), new Vector3(2, 2.5f, 0),
                out collisionAxis);
            Assert.IsNull(collisionAxis);
            Assert.IsNull(distance);
        }

        // Prüft das Entlang schlittern an einer Wand
        [TestMethod]
        public void SlidingWall()
        {
            // ###
            //  3#
            //  2#
            // 1

            List<Index3> blocks = new List<Index3>();
            blocks.Add(new Index3(2, 2, 1));
            blocks.Add(new Index3(3, 2, 1));
            blocks.Add(new Index3(4, 2, 1));
            blocks.Add(new Index3(4, 3, 1));
            blocks.Add(new Index3(4, 4, 1));

            BoundingBox player = new BoundingBox(new Vector3(2, 5, 1), new Vector3(3, 6, 1));
            Vector3 move = new Vector3(0.75f, -0.75f, 0);
            Axis? collisionAxis;
            float? distance;

            // Step 1 (2/5 -> 2.75/4.25 (keine Kollision)
            foreach (var pos in blocks)
            {
                distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);
                Assert.IsNull(collisionAxis);
                Assert.IsNull(distance);
            }

            // Step 2 (2.75/4.25 -> 3.5/3.5 (kollision X) -> 3.0/3.5
            player = new BoundingBox(player.Min + move, player.Max + move);
            foreach (var pos in blocks)
            {
                distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);

                if (pos == new Index3(4, 3, 1) || pos == new Index3(4, 4, 1))
                {
                    Assert.AreEqual(Axis.X, collisionAxis);
                    Assert.AreEqual(1f/3f, distance);
                }
                else
                {
                    Assert.IsNull(collisionAxis);
                    Assert.IsNull(distance);
                }
            }

            // Step 3 (3.0/3.5 -> 3.75/2.75 (Kollision X & Y) -> 3/3
            player = new BoundingBox(new Vector3(3, 3.5f, 1), new Vector3(4, 4.5f, 1));
            foreach (var pos in blocks)
            {
                distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);

                if (pos == new Index3(4, 3, 1) || pos == new Index3(4, 4, 1))
                {
                    Assert.AreEqual(Axis.X, collisionAxis);
                    Assert.AreEqual(0f, distance);
                }
                else if (pos == new Index3(2, 2, 1) || pos == new Index3(3, 2, 1) || pos == new Index3(4, 2, 1))
                {
                    Assert.AreEqual(Axis.Y, collisionAxis);
                    Assert.AreEqual(2f/3f, distance);
                }
                else
                {
                    Assert.IsNull(collisionAxis);
                    Assert.IsNull(distance);
                }
            }

            // Step 4 (freeze) 3/3 -> 3.75/2.25 (Kollision X & Y) -> 3/3
            player = new BoundingBox(new Vector3(3, 3, 1), new Vector3(4, 3, 1));
            foreach (var pos in blocks)
            {
                distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);

                if (pos == new Index3(4, 2, 1) || pos == new Index3(4, 3, 1))
                {
                    Assert.AreEqual(Axis.X, collisionAxis);
                    Assert.AreEqual(0f, distance);
                }
                else if (pos == new Index3(2, 2, 1) || pos == new Index3(3, 2, 1))
                {
                    Assert.AreEqual(Axis.Y, collisionAxis);
                    Assert.AreEqual(0, distance);
                }
                else
                {
                    Assert.IsNull(collisionAxis);
                    Assert.IsNull(distance);
                }
            }
        }
    }
}