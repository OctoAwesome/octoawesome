using engenious;
using System.Collections.Generic;
using NUnit.Framework;

namespace OctoAwesome.Tests
{
    public class BlockIntersectTests
    {
        // TODO: adapt tests to new implementations
        ///// <summary>
        ///// Tests behaviour when the selection block is too far away from the player.
        ///// </summary>
        //[Test]
        //public void BlockIntersectFromOutside()
        //{
        //    BoundingBox player = new BoundingBox(new Vector3(5, 20, 30), new Vector3(6, 21, 31));

        //    // X-Axis too far away from on the left side (No movement)
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0),
        //        new Index3(10, 20, 30), player, new Vector3(), out collisionAxis);
        //    Assert.Null(collisionAxis);
        //    Assert.Null( distance);

        //    // X-Axis too far away from on the left side (Movement to the left)
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0),
        //        new Index3(10, 20, 30), player, new Vector3(-4, 0, 0), out collisionAxis);
        //    Assert.Null( collisionAxis);
        //    Assert.Null( distance);

        //    // X-Axis too far away from on the left side (Movement to the right)
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0),
        //        new Index3(10, 20, 30), player, new Vector3(4, 0, 0), out collisionAxis);
        //    Assert.Null(collisionAxis);
        //    Assert.Null(distance);
        //}

        ///// <summary>
        ///// Tests intersection handling on complete overlap
        ///// </summary>
        //[Test]
        //public void BlockIntersectPreviousCollision()
        //{
        //    // X-Axis overlaps already on the left (No movement)
        //    // !!! No chance of resolution - just ;)
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(
        //        block.GetCollisionBoxes(null, 0, 0, 0),
        //        new Index3(10, 20, 30),
        //        new BoundingBox(new Vector3(10, 20, 30), new Vector3(11, 21, 31)), new Vector3(), out collisionAxis);
        //    Assert.Null(collisionAxis);
        //    Assert.Null(distance);

        //    // X-Axis overlaps already on the left (movement to the left)
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 20, 30), new Vector3(11, 21, 31)), new Vector3(-2, 0, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(-0.5f, distance);

        //    // X-Axis overlaps already on the left (movement to the right)
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 20, 30), new Vector3(11, 21, 31)), new Vector3(2, 0, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(-0.5f, distance);
        //}

        ///// <summary>
        ///// Tests, if the correct axes and distance was determined
        ///// </summary>
        //[Test]
        //public void BlockIntersectAxisCheck()
        //{
        //    // Collision check on +X axis
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(8, 20, 30), new Vector3(9, 21, 31)), new Vector3(2, 0, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Collision check on -X axis
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(12, 20, 30), new Vector3(13, 21, 31)), new Vector3(-2, 0, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Collision check on +Y axis
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 18, 30), new Vector3(11, 19, 31)), new Vector3(0, 2, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.Y, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Collision check on -Y axis
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 22, 30), new Vector3(11, 23, 31)), new Vector3(0, -2, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.Y, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Collision check on +Z axis
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 20, 28), new Vector3(11, 21, 29)), new Vector3(0, 0, 2), out collisionAxis);
        //    Assert.AreEqual(Axis.Z, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Collision check on -Z axis
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 20, 32), new Vector3(11, 21, 33)), new Vector3(0, 0, -2), out collisionAxis);
        //    Assert.AreEqual(Axis.Z, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);
        //}

        ///// <summary>
        ///// Tests the axis prioritisation
        ///// </summary>
        //[Test]
        //public void BlockIntersectDistanceCheck()
        //{
        //    // Movement from the top left with collision on X
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(8, 18, 28), new Vector3(9, 19, 29)), new Vector3(2, 3, 3), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Movement from top left with collision on Y
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(8, 18, 28), new Vector3(9, 19, 29)), new Vector3(3, 2, 3), out collisionAxis);
        //    Assert.AreEqual(Axis.Y, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Movement from top left with collision on Z
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(8, 18, 28), new Vector3(9, 19, 29)), new Vector3(3, 3, 2), out collisionAxis);
        //    Assert.AreEqual(Axis.Z, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Movement from bottom right with collision on X
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(12, 22, 32), new Vector3(13, 23, 33)), new Vector3(-2, -3, -3), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Movement from bottom right with collision on Y
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(12, 22, 32), new Vector3(13, 23, 33)), new Vector3(-3, -2, -3), out collisionAxis);
        //    Assert.AreEqual(Axis.Y, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // Movement from bottom right with collision on Z
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(12, 22, 32), new Vector3(13, 23, 33)), new Vector3(-3, -3, -2), out collisionAxis);
        //    Assert.AreEqual(Axis.Z, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);
        //}

        ///// <summary>
        ///// Tests if penetrated axis are ignored.
        ///// </summary>
        //[Test]
        //public void BlockIntersectSliding()
        //{
        //    // X
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(8, 20.5f, 30), new Vector3(9, 21.5f, 31)), new Vector3(2, 0.5f, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);
        //}

        ///// <summary>
        ///// Tests if penetration is intercepted.
        ///// </summary>
        //public void BlockIntersectDiffusion()
        //{
        //    // Despite complete penetration a collision must be found
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(9, 19, 29), new Vector3(10, 20, 30)), new Vector3(5, 5, 5), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(1f / 3f, distance);
        //}

        ///// <summary>
        ///// Tests a collision with an edge, which is not the move edge of the player.
        ///// </summary>
        //[Test]
        //public void OppositeCornerIntersect()
        //{
        //    // x
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(8, 20, 30), new Vector3(9, 21, 31)), new Vector3(2, 1.5f, 0), out collisionAxis);
        //    Assert.AreEqual(Axis.X, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // y
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 18, 30), new Vector3(11, 19, 31)), new Vector3(0, 2f, 1.5f), out collisionAxis);
        //    Assert.AreEqual(Axis.Y, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);

        //    // z
        //    distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(10, 20, 28), new Vector3(11, 21, 29)), new Vector3(0, 1.5f, 2), out collisionAxis);
        //    Assert.AreEqual(Axis.Z, collisionAxis);
        //    Assert.AreEqual(0.5f, distance);
        //}

        ///// <summary>
        ///// Tests behaviour of collision for blocks which barely move past each other on their edges
        ///// </summary>
        //[Test]
        //public void NonContactCornerIntersect()
        //{
        //    // x
        //    Axis? collisionAxis;
        //    float? distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), new Index3(10, 20, 30), new BoundingBox(new Vector3(8, 20, 30), new Vector3(9, 21, 31)), new Vector3(2, 2.5f, 0), out collisionAxis);
        //    Assert.Null(collisionAxis);
        //    Assert.Null(distance);
        //}

        //// Tests slithering along a wall
        //[Test]
        //public void SlidingWall()
        //{
        //    // ###
        //    //  3#
        //    //  2#
        //    // 1

        //    List<Index3> blocks = new List<Index3>();
        //    blocks.Add(new Index3(2, 2, 1));
        //    blocks.Add(new Index3(3, 2, 1));
        //    blocks.Add(new Index3(4, 2, 1));
        //    blocks.Add(new Index3(4, 3, 1));
        //    blocks.Add(new Index3(4, 4, 1));

        //    BoundingBox player = new BoundingBox(new Vector3(2, 5, 1), new Vector3(3, 6, 1));
        //    Vector3 move = new Vector3(0.75f, -0.75f, 0);
        //    Axis? collisionAxis;
        //    float? distance;

        //    // Step 1 (2/5 -> 2.75/4.25 (No collision)
        //    foreach (var pos in blocks)
        //    {
        //        distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);
        //        Assert.Null(collisionAxis);
        //        Assert.Null(distance);
        //    }

        //    // Step 2 (2.75/4.25 -> 3.5/3.5 (Collision X) -> 3.0/3.5
        //    player = new BoundingBox(player.Min + move, player.Max + move);
        //    foreach (var pos in blocks)
        //    {
        //        distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);

        //        if (pos == new Index3(4, 3, 1) || pos == new Index3(4, 4, 1))
        //        {
        //            Assert.AreEqual(Axis.X, collisionAxis);
        //            Assert.AreEqual(1f / 3f, distance);
        //        }
        //        else
        //        {
        //            Assert.Null(collisionAxis);
        //            Assert.Null(distance);
        //        }
        //    }

        //    // Step 3 (3.0/3.5 -> 3.75/2.75 (Collision X & Y) -> 3/3
        //    player = new BoundingBox(new Vector3(3, 3.5f, 1), new Vector3(4, 4.5f, 1));
        //    foreach (var pos in blocks)
        //    {
        //        distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);

        //        if (pos == new Index3(4, 3, 1) || pos == new Index3(4, 4, 1))
        //        {
        //            Assert.AreEqual(Axis.X, collisionAxis);
        //            Assert.AreEqual(0f, distance);
        //        }
        //        else if (pos == new Index3(2, 2, 1) || pos == new Index3(3, 2, 1) || pos == new Index3(4, 2, 1))
        //        {
        //            Assert.AreEqual(Axis.Y, collisionAxis);
        //            Assert.AreEqual(2f / 3f, distance);
        //        }
        //        else
        //        {
        //            Assert.Null(collisionAxis);
        //            Assert.Null(distance);
        //        }
        //    }

        //    // Step 4 (freeze) 3/3 -> 3.75/2.25 (Collision X & Y) -> 3/3
        //    player = new BoundingBox(new Vector3(3, 3, 1), new Vector3(4, 3, 1));
        //    foreach (var pos in blocks)
        //    {
        //        distance = Block.Intersect(block.GetCollisionBoxes(null, 0, 0, 0), pos, player, move, out collisionAxis);

        //        if (pos == new Index3(4, 2, 1) || pos == new Index3(4, 3, 1))
        //        {
        //            Assert.AreEqual(Axis.X, collisionAxis);
        //            Assert.AreEqual(0f, distance);
        //        }
        //        else if (pos == new Index3(2, 2, 1) || pos == new Index3(3, 2, 1))
        //        {
        //            Assert.AreEqual(Axis.Y, collisionAxis);
        //            Assert.AreEqual(0, distance);
        //        }
        //        else
        //        {
        //            Assert.Null(collisionAxis);
        //            Assert.Null(distance);
        //        }
        //    }
        //}
    }
}
