using System;
using System.Collections.Generic;
using System.Linq;
using engenious;
using OctoAwesome.Ecs;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.Systems
{
    [SystemConfiguration(After = new [] { "GravitySystem", "JumpingSystem", "LookMovementSystem" })]
    public sealed class CollidingMovementSystem : BaseSystemR3<PositionComponent, MoveableComponent, CollisionComponent>
    {
        public CollidingMovementSystem(EntityManager manager) : base(manager) { }

        protected override void Update(Entity e, PositionComponent r1, MoveableComponent r2, CollisionComponent r3)
        {
            Vector3 exforce = r2.Force;

            var elapsedTime = Manager.GameTime.ElapsedGameTime;

            Vector3 externalPower = ((exforce * exforce) / (2 * r2.Mass)) * (float)elapsedTime.TotalSeconds ;
            externalPower *= new Vector3(Math.Sign(exforce.X), Math.Sign(exforce.Y), Math.Sign(exforce.Z));
            externalPower += r2.Power;

            Vector3 friction = new Vector3(1, 1, 0.1f) * PlayerComponent.FRICTION;

            var velocityChange = (2.0f/r2.Mass*(externalPower - friction*r2.Velocity))*(float) elapsedTime.TotalSeconds;
           
            r2.Velocity += new Vector3(
                (float) (velocityChange.X < 0 ? -Math.Sqrt(-velocityChange.X) : Math.Sqrt(velocityChange.X)),
                (float) (velocityChange.Y < 0 ? -Math.Sqrt(-velocityChange.Y) : Math.Sqrt(velocityChange.Y)),
                (float) (velocityChange.Z < 0 ? -Math.Sqrt(-velocityChange.Z) : Math.Sqrt(velocityChange.Z))
            );

            var move = r2.Velocity*(float) elapsedTime.TotalSeconds;

            int minx = (int)Math.Floor(Math.Min(r1.Coordinate.BlockPosition.X - r1.Radius, r1.Coordinate.BlockPosition.X - r1.Radius + move.X));
            int maxx = (int)Math.Ceiling(Math.Max(r1.Coordinate.BlockPosition.X + r1.Radius, r1.Coordinate.BlockPosition.X + r1.Radius + move.X));
            int miny = (int)Math.Floor(Math.Min(r1.Coordinate.BlockPosition.Y - r1.Radius, r1.Coordinate.BlockPosition.Y - r1.Radius + move.Y));
            int maxy = (int)Math.Ceiling(Math.Max(r1.Coordinate.BlockPosition.Y + r1.Radius, r1.Coordinate.BlockPosition.Y + r1.Radius + move.Y));
            int minz = (int)Math.Floor(Math.Min(r1.Coordinate.BlockPosition.Z, r1.Coordinate.BlockPosition.Z + move.Z));
            int maxz = (int)Math.Ceiling(Math.Max(r1.Coordinate.BlockPosition.Z + r1.Height, r1.Coordinate.BlockPosition.Z + r1.Height + move.Z));

            var playerplanes = CollisionPlane.GetPlayerCollisionPlanes(r1, r2.Velocity).ToList();

            bool abort = false;

            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        move = r2.Velocity * (float)elapsedTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + r1.Coordinate.GlobalBlockIndex;
                        ushort block = r2.LocalChunkCache.GetBlock(blockPos);

                        if (block == 0)
                            continue;
                        
                        var blockplane = CollisionPlane.GetBlockCollisionPlanes(pos, r2.Velocity).ToList();

                        var planes = from pp in playerplanes
                                     from bp in blockplane
                                     where CollisionPlane.Intersect(bp, pp)
                                     let distance = CollisionPlane.GetDistance(bp, pp)
                                     where CollisionPlane.CheckDistance(distance, move)
                                     select new { BlockPlane = bp, PlayerPlane = pp, Distance = distance };

                        foreach (var plane in planes)
                        {

                            var subvelocity = (plane.Distance / (float)elapsedTime.TotalSeconds);
                            var diff = r2.Velocity - subvelocity;

                            float vx;
                            float vy;
                            float vz;

                            if (plane.BlockPlane.normal.X != 0 && (r2.Velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 || r2.Velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                vx = subvelocity.X;
                            else
                                vx = r2.Velocity.X;

                            if (plane.BlockPlane.normal.Y != 0 && (r2.Velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 || r2.Velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                vy = subvelocity.Y;
                            else
                                vy = r2.Velocity.Y;

                            if (plane.BlockPlane.normal.Z != 0 && (r2.Velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 || r2.Velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                vz = subvelocity.Z;
                            else
                                vz = r2.Velocity.Z;

                            r2.Velocity = new Vector3(vx, vy, vz);

                            if (vx == 0 && vy == 0 && vz == 0)
                            {
                                abort = true;
                                break;
                            }
                        }
                    }
                }
            }
            r1.OnGround = r2.Velocity.Z == 0f;
            r2.Force = Vector3.Zero;
            r2.Power = Vector3.Zero;
            var position = r1.Coordinate + r2.Velocity * (float)elapsedTime.TotalSeconds;
            position.NormalizeChunkIndexXY(r1.Planet.Size);
            r1.Coordinate = position;

        }

        public struct CollisionPlane
        {
            /// <summary>
            /// Normalenvector der Fläche
            /// </summary>
            public Vector3 normal;
            /// <summary>
            /// Mittelpunkt der Fläche
            /// </summary>
            public Vector3 pos;
            /// <summary>
            /// Erste Ecke der Fläche
            /// </summary>
            public Vector3 edgepos1;
            /// <summary>
            /// Zweite Fläche der Ecke
            /// </summary>
            public Vector3 edgepos2;

            /// <summary>
            /// Konstruktur
            /// </summary>
            /// <param name="pos1">Ecke 1</param>
            /// <param name="pos2">Ecke 2</param>
            /// <param name="normal">Normalenvektor</param>
            public CollisionPlane(Vector3 pos1, Vector3 pos2, Vector3 normal)
            {
                this.normal = normal;
                this.edgepos1 = pos1;
                this.edgepos2 = pos2;

                pos = (pos2 - pos1) / 2f + pos1;
            }

            /// <summary>
            /// Gibt alle möglichen Flächen eines Blockes zurück
            /// </summary>
            /// <param name="pos">Position des Blockes</param>
            /// <param name="movevector">Bewegungsvector</param>
            /// <returns>Liste aller beteiligten Flächen</returns>
            public static IEnumerable<CollisionPlane> GetBlockCollisionPlanes(Index3 pos, Vector3 movevector)
            {
                //Ebene X
                if (movevector.X > 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X, pos.Y, pos.Z),
                        new Vector3(pos.X, pos.Y + 1f, pos.Z + 1f),
                        new Vector3(-1, 0, 0));
                }
                else if (movevector.X < 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X + 1f, pos.Y, pos.Z),
                        new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z + 1f),
                        new Vector3(1, 0, 0));
                }

                //Ebene Y
                if (movevector.Y > 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X, pos.Y, pos.Z),
                        new Vector3(pos.X + 1f, pos.Y, pos.Z + 1f),
                        new Vector3(0, -1, 0));
                }
                else if (movevector.Y < 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X, pos.Y + 1f, pos.Z),
                        new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z + 1f),
                        new Vector3(0, 1, 0));
                }

                //Ebene Z
                if (movevector.Z > 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X, pos.Y, pos.Z),
                        new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z),
                        new Vector3(0, 0, -1));
                }
                else if (movevector.Z < 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X, pos.Y, pos.Z + 1f),
                        new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z + 1f),
                        new Vector3(0, 0, 1));
                }


            }

            /// <summary>
            /// Gibt alle Flächen eines Spielers zurück
            /// </summary>
            /// <param name="player">Spieler</param>
            /// <param name="invertvelocity">Gibt an ob die geschwindigkeit invertiert werden soll</param>
            /// <returns>Alle beteiligten Flächen des Spielers</returns>
            public static IEnumerable<CollisionPlane> GetPlayerCollisionPlanes(PositionComponent pc, Vector3 velocity, bool invertvelocity = true)
            {
                var pos = pc.Coordinate.BlockPosition;
                var vel = invertvelocity ? -velocity : velocity;

                //Ebene X
                if (vel.X > 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X - pc.Radius, pos.Y - pc.Radius, pos.Z),
                        new Vector3(pos.X - pc.Radius, pos.Y + pc.Radius, pos.Z + pc.Height),
                        new Vector3(-1, 0, 0));
                }
                else if (vel.X < 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X + pc.Radius, pos.Y - pc.Radius, pos.Z),
                        new Vector3(pos.X + pc.Radius, pos.Y + pc.Radius, pos.Z + pc.Height),
                        new Vector3(1, 0, 0));
                }

                //Ebene Y
                if (vel.Y > 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X - pc.Radius, pos.Y - pc.Radius, pos.Z),
                        new Vector3(pos.X + pc.Radius, pos.Y - pc.Radius, pos.Z + pc.Height),
                        new Vector3(0, -1, 0));
                }
                else if (vel.Y < 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X - pc.Radius, pos.Y + pc.Radius, pos.Z),
                        new Vector3(pos.X + pc.Radius, pos.Y + pc.Radius, pos.Z + pc.Height),
                        new Vector3(0, 1, 0));
                }

                //Ebene Z
                if (vel.Z > 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X - pc.Radius, pos.Y - pc.Radius, pos.Z),
                        new Vector3(pos.X + pc.Radius, pos.Y + pc.Radius, pos.Z),
                        new Vector3(0, 0, -1));
                }
                else if (vel.Z < 0)
                {
                    yield return new CollisionPlane(
                        new Vector3(pos.X - pc.Radius, pos.Y - pc.Radius, pos.Z + pc.Height),
                        new Vector3(pos.X + pc.Radius, pos.Y + pc.Radius, pos.Z + pc.Height),
                        new Vector3(0, 0, 1));
                }
            }

            /// <summary>
            /// Gibt zurück ob zwei Flächen miteinander Kollidieren können(Achtung noch keine gedrehten Flächen)
            /// </summary>
            /// <param name="p1">Fläche 1</param>
            /// <param name="p2">Fläche 2</param>
            /// <returns>Ergebnis der Kollisions</returns>
            public static bool Intersect(CollisionPlane p1, CollisionPlane p2)
            {
                //TODO: Erweitern auf schräge Fläche
                var vec = p1.normal * p2.normal;

                bool result = false;

                if (vec.X < 0)
                {
                    var ry = (p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y) ||
                             (p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y) ||
                             (p1.edgepos1.Y > p2.edgepos1.Y && p1.edgepos1.Y < p2.edgepos2.Y) ||
                             (p1.edgepos2.Y < p2.edgepos2.Y && p1.edgepos2.Y > p2.edgepos1.Y);

                    var rz = (p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z) ||
                             (p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z) ||
                             (p1.edgepos1.Z > p2.edgepos1.Z && p1.edgepos1.Z < p2.edgepos2.Z) ||
                             (p1.edgepos2.Z < p2.edgepos2.Z && p1.edgepos2.Z > p2.edgepos1.Z);

                    result = rz && ry;
                }
                else if (vec.Y < 0)
                {
                    var rx = (p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X) ||
                             (p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X) ||
                             (p1.edgepos1.X > p2.edgepos1.X && p1.edgepos1.X < p2.edgepos2.X) ||
                             (p1.edgepos2.X < p2.edgepos2.X && p1.edgepos2.X > p2.edgepos1.X);

                    var rz = (p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z) ||
                             (p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z) ||
                             (p1.edgepos1.Z > p2.edgepos1.Z && p1.edgepos1.Z < p2.edgepos2.Z) ||
                             (p1.edgepos2.Z < p2.edgepos2.Z && p1.edgepos2.Z > p2.edgepos1.Z);


                    result = rx && rz;
                }
                else if (vec.Z < 0)
                {
                    var rx = (p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X) ||
                            (p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X) ||
                            (p1.edgepos1.X > p2.edgepos1.X && p1.edgepos1.X < p2.edgepos2.X) ||
                            (p1.edgepos2.X < p2.edgepos2.X && p1.edgepos2.X > p2.edgepos1.X);

                    var ry = (p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y) ||
                             (p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y) ||
                             (p1.edgepos1.Y > p2.edgepos1.Y && p1.edgepos1.Y < p2.edgepos2.Y) ||
                             (p1.edgepos2.Y < p2.edgepos2.Y && p1.edgepos2.Y > p2.edgepos1.Y);

                    result = rx && ry;
                }

                return result;
            }

            /// <summary>
            /// Gibt den Abstand zweier Flächen zurück(Mittelpunkt zu Mittelpunkt)
            /// </summary>
            /// <param name="p1">Fläche 1</param>
            /// <param name="p2">Fläche 2</param>
            /// <returns>Abstand der Flächen zueinander</returns>
            public static Vector3 GetDistance(CollisionPlane p1, CollisionPlane p2)
            {
                var alpha = p1.normal * p2.normal;


                var dvector = new Vector3();
                dvector.X = alpha.X != 0 ? 1 : 0;
                dvector.Y = alpha.Y != 0 ? 1 : 0;
                dvector.Z = alpha.Z != 0 ? 1 : 0;

                var distance = (p1.pos - p2.pos) * dvector;

                return distance;
            }

            /// <summary>
            /// Überprüft ob Vektor 2 größer als Vektor 1 ist
            /// </summary>
            /// <param name="d1">Vektor 1</param>
            /// <param name="d2">Vektor 2</param>
            /// <returns>Ergebnis</returns>
            public static bool CheckDistance(Vector3 d1, Vector3 d2)
            {

                if (d1.X == 0 || d1.Y == 0 || d1.Z == 0)
                    return true;

                var diff = d1 - d2;

                var rx = d1.X > 0 ? diff.X < 0 : diff.X > 0;
                var ry = d1.Y > 0 ? diff.Y < 0 : diff.Y > 0;
                var rz = d1.Z > 0 ? diff.Z < 0 : diff.Z > 0;


                return rx || ry || rz;
            }


        }
    }
}
