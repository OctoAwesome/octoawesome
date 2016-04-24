using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public struct CollisionPlane
    {
        public Vector3 normal;
        public Vector3 pos;
        public Vector3 edgepos1;
        public Vector3 edgepos2;


        public CollisionPlane(Vector3 pos1,Vector3 pos2, Vector3 normal)
        {
            this.normal = normal;
            this.edgepos1 = pos1;
            this.edgepos2 = pos2;

            pos = (pos2 - pos1 ) /2f + pos1;
        }

        public static IEnumerable<CollisionPlane> GetBlockCollisionPlanes(Index3 pos, Vector3 movevector)
        {
            //Ebene X
            if (movevector.X > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X-0.5f,pos.Y -0.5f, pos.Z - 0.5f),
                    new Vector3(pos.X - 0.5f, pos.Y + 0.5f, pos.Z + 0.5f), 
                    new Vector3(-1, 0, 0));
            }
            else if (movevector.X < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X + 0.5f, pos.Y - 0.5f, pos.Z - 0.5f),
                    new Vector3(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f),
                    new Vector3(1, 0, 0));
            }

            //Ebene Y
            if (movevector.Y > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - 0.5f, pos.Y-0.5f, pos.Z-0.5f),
                    new Vector3(pos.X + 0.5f, pos.Y - 0.5f, pos.Z + 0.5f),
                    new Vector3(0, -1, 0));
            }
            else if (movevector.Y < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - 0.5f, pos.Y + 0.5f, pos.Z -0.5f),
                    new Vector3(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f),
                    new Vector3(0, 1, 0));
            }

            //Ebene Z
            if (movevector.Z > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - 0.5f, pos.Y - 0.5f , pos.Z - 0.5f),
                    new Vector3(pos.X + 0.5f, pos.Y + 0.5f , pos.Z - 0.5f),
                    new Vector3(0,0, -1));
            }
            else if (movevector.Z < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - 0.5f, pos.Y - 0.5f , pos.Z + 0.5f),
                    new Vector3(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f),
                    new Vector3(0, 0, 1));
            }


        }

        public static IEnumerable<CollisionPlane> GetPlayerCollisionPlanes(Player player,bool invertvelocity = true)
        {
            var pos = player.Position.BlockPosition;
            var vel = invertvelocity ? -1 * player.Velocity : player.Velocity;

            //Ebene X
            if (vel.X > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - player.Radius, pos.Y - player.Radius, pos.Z),
                    new Vector3(pos.X - player.Radius, pos.Y + player.Radius, pos.Z + player.Height),
                    new Vector3(-1, 0, 0));
            }
            else if (vel.X < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X + player.Radius, pos.Y - player.Radius, pos.Z - player.Height),
                    new Vector3(pos.X + player.Radius, pos.Y + player.Radius, pos.Z + player.Height),
                    new Vector3(1, 0, 0));
            }

            //Ebene Y
            if (vel.Y > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - player.Radius, pos.Y - player.Radius, pos.Z ),
                    new Vector3(pos.X + player.Radius, pos.Y - player.Radius, pos.Z + player.Height),
                    new Vector3(0, -1, 0));
            }
            else if (vel.Y < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - player.Radius, pos.Y + player.Radius, pos.Z),
                    new Vector3(pos.X + player.Radius, pos.Y + player.Radius, pos.Z + player.Height),
                    new Vector3(0, 1, 0));
            }

            //Ebene Z
            if (vel.Z > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - player.Radius, pos.Y - player.Radius, pos.Z - 0.5f),
                    new Vector3(pos.X + player.Radius, pos.Y + player.Radius, pos.Z - 0.5f),
                    new Vector3(0, 0, -1));
            }
            else if (vel.Z < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - player.Radius, pos.Y - player.Radius , pos.Z + player.Height - 0.5f),
                    new Vector3(pos.X + player.Radius, pos.Y + player.Radius, pos.Z + player.Height - 0.5f),
                    new Vector3(0, 0, 1));
            }
        }

        public static bool Intersect(CollisionPlane p1, CollisionPlane p2)
        {
            var result = Vector3.Dot(p1.normal, p2.normal) < 0;

            if (result)
            {
                if (p1.normal.X != 0 && p2.normal.X != 0)
                {
                    
                    var ry = (p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y) || (p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y);
                    var rz = (p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z) || (p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z);

                    result = rz && ry;
                }
                else if (p1.normal.Y != 0 && p2.normal.Y != 0)
                {
                    var rx = (p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X) || (p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X);
                    var rz = (p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z) || (p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z);

                    result = rx && rz;
                }
                else if (p1.normal.Z != 0 && p2.normal.Z != 0)
                {
                    var rx = (p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X) || (p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X);
                    var ry = (p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y) || (p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y);

                    result = rx && ry;
                }
            }

            return result;
        }

        public static Vector3 GetDistance(CollisionPlane p1, CollisionPlane p2)
        {
            var alpha = p1.normal * p2.normal;


            var dvector = new Vector3();
            dvector.X = alpha.X != 0 ? 1 : 0;
            dvector.Y = alpha.Y != 0 ? 1 : 0;
            dvector.Z = alpha.Z != 0 ? 1 : 0;

            var distance = p1.pos * dvector - p2.pos * dvector;

            return distance;
        }
        public static bool CheckDistance(Vector3 d1, Vector3 d2)
        {

            var diff = d1 - d2;

            var rx = d1.X > 0 ? diff.X < 0 : diff.X > 0;
            var ry = d1.Y > 0 ? diff.Y < 0 : diff.Y > 0;
            var rz = d1.Z > 0 ? diff.Z < 0 : diff.Z > 0;


            return rx || ry || rz;
        }

       
    }
}
