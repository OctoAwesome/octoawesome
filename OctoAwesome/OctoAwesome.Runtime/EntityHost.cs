using Microsoft.Xna.Framework;
using OctoAwesome.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Host für Entitäten.
    /// </summary>
    public class EntityHost
    {
        private readonly float Gap = 0.001f;

        /// <summary>
        /// Der Planet, auf dem sich die Entität befindet.
        /// </summary>
        protected IPlanet planet;

        /// <summary>
        /// Der lokale Chunk Cache für die Entität.
        /// </summary>
        protected ILocalChunkCache localChunkCache;

        private Index3 _oldIndex;

        /// <summary>
        /// Gibt an, ob die Entität bereit ist.
        /// </summary>
        public bool ReadyState { get; private set; }

        /// <summary>
        /// Die gehostete Entität.
        /// </summary>
        public ControllableEntity Entity { get; private set; }

        /// <summary>
        /// Erzeugt einen neuen Host für Entitäten.
        /// </summary>
        /// <param name="entity">Die zu hostende Entität.</param>
        public EntityHost(ControllableEntity entity)
        {
            Entity = entity;
            _oldIndex = Entity.Position.ChunkIndex;
            planet = ResourceManager.Instance.GetPlanet(Entity.Position.Planet);
            localChunkCache = new LocalChunkCache(ResourceManager.Instance.GlobalChunkCache, 2, 1);
            ReadyState = false;
        }

        /// <summary>
        /// Initialisiert den lokalen Chunk Cache der Entität.
        /// </summary>
        /// <param name="successCallback">Callback, das ausgeführt wird, wenn der <see cref="LocalChunkCache.SetCenter(IPlanet, Index2, Action{bool})"/>-Aufruf abgeschlossen ist.</param>
        public virtual void Initialize(Action successCallback)
        {
            localChunkCache.SetCenter(planet, new Index2(Entity.Position.ChunkIndex), (success) =>
            {
                ReadyState = success;
                if (successCallback != null)
                    successCallback();
            });

            if (Entity is PermanentEntity)
            {
                ResourceManager.Instance.EntityCache.Subscribe(planet, new Index2(Entity.Position.ChunkIndex), 
                    (Entity as PermanentEntity).ActivationRange);
            }
        }

        int lastJump = 0;

        /// <summary>
        /// Wird von Update aufgerufen.
        /// </summary>
        /// <param name="frameTime"></param>
        protected virtual void BeforeUpdate(GameTime frameTime)
        {
            bool jump = false;
            if ((int)frameTime.TotalGameTime.TotalSeconds > lastJump)
            {
                jump = true;
                lastJump = (int)frameTime.TotalGameTime.TotalSeconds;
            }

            Entity.Velocity += PhysicalUpdate(Vector3.Zero, frameTime.ElapsedGameTime, true, jump);
        }

        /// <summary>
        /// Führt Berechnungen der Bewegung usw. aus.
        /// </summary>
        /// <param name="frameTime">Die aktuelle GameTime.</param>
        public virtual void Update(GameTime frameTime)
        {
            BeforeUpdate(frameTime);

            #region Playerbewegung

            Vector3 move = Entity.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds;

            Entity.OnGround = false;
            bool collision = false;
            int loop = 0;

            do
            {
                int minx = (int)Math.Floor(Math.Min(
                    Entity.Position.BlockPosition.X - Entity.Radius,
                    Entity.Position.BlockPosition.X - Entity.Radius + move.X));
                int maxx = (int)Math.Floor(Math.Max(
                    Entity.Position.BlockPosition.X + Entity.Radius,
                    Entity.Position.BlockPosition.X + Entity.Radius + move.X));
                int miny = (int)Math.Floor(Math.Min(
                    Entity.Position.BlockPosition.Y - Entity.Radius,
                    Entity.Position.BlockPosition.Y - Entity.Radius + move.Y));
                int maxy = (int)Math.Floor(Math.Max(
                    Entity.Position.BlockPosition.Y + Entity.Radius,
                    Entity.Position.BlockPosition.Y + Entity.Radius + move.Y));
                int minz = (int)Math.Floor(Math.Min(
                    Entity.Position.BlockPosition.Z,
                    Entity.Position.BlockPosition.Z + move.Z));
                int maxz = (int)Math.Floor(Math.Max(
                    Entity.Position.BlockPosition.Z + Entity.Height,
                    Entity.Position.BlockPosition.Z + Entity.Height + move.Z));

                // Relative PlayerBox
                BoundingBox playerBox = new BoundingBox(
                    new Vector3(
                        Entity.Position.BlockPosition.X - Entity.Radius,
                        Entity.Position.BlockPosition.Y - Entity.Radius,
                        Entity.Position.BlockPosition.Z),
                    new Vector3(
                        Entity.Position.BlockPosition.X + Entity.Radius,
                        Entity.Position.BlockPosition.Y + Entity.Radius,
                        Entity.Position.BlockPosition.Z + Entity.Height));

                collision = false;
                float min = 1f;
                Axis minAxis = Axis.None;

                for (int z = minz; z <= maxz; z++)
                {
                    for (int y = miny; y <= maxy; y++)
                    {
                        for (int x = minx; x <= maxx; x++)
                        {
                            Index3 pos = new Index3(x, y, z);
                            Index3 blockPos = pos + Entity.Position.GlobalBlockIndex;
                            ushort block = localChunkCache.GetBlock(blockPos);
                            if (block == 0)
                                continue;

                            Axis? localAxis;
                            IBlockDefinition blockDefinition = DefinitionManager.Instance.GetBlockDefinitionByIndex(block);
                            float? moveFactor = Block.Intersect(
                                blockDefinition.GetCollisionBoxes(localChunkCache, blockPos.X, blockPos.Y, blockPos.Z),
                                pos, playerBox, move, out localAxis);

                            if (moveFactor.HasValue && moveFactor.Value < min)
                            {
                                collision = true;
                                min = moveFactor.Value;
                                minAxis = localAxis.Value;
                            }
                        }
                    }
                }

                Entity.Position += (move * min);
                move *= (1f - min);
                switch (minAxis)
                {
                    case Axis.X:
                        Entity.Velocity *= new Vector3(0, 1, 1);
                        Entity.Position += new Vector3(move.X > 0 ? -Gap : Gap, 0, 0);
                        move.X = 0f;
                        break;
                    case Axis.Y:
                        Entity.Velocity *= new Vector3(1, 0, 1);
                        Entity.Position += new Vector3(0, move.Y > 0 ? -Gap : Gap, 0);
                        move.Y = 0f;
                        break;
                    case Axis.Z:
                        Entity.OnGround = true;
                        Entity.Velocity *= new Vector3(1, 1, 0);
                        Entity.Position += new Vector3(0, 0, move.Z > 0 ? -Gap : Gap);
                        move.Z = 0f;
                        break;
                }

                // Koordinate normalisieren (Rundwelt)
                Coordinate position = Entity.Position;
                position.NormalizeChunkIndexXY(planet.Size);

                //Beam me up
                //KeyboardState ks = Keyboard.GetState();
                //if (ks.IsKeyDown(Keys.P))
                //{
                //    position = position + new Vector3(0, 0, 10);
                //}

                // Chunkwechsel signalisieren
                if (Entity is PermanentEntity)
                {
                    // TODO: Prüfung auf PermanentEntity noch notwendig?
                    if (Entity.Position.ChunkIndex.X != position.ChunkIndex.X ||
                        Entity.Position.ChunkIndex.Y != position.ChunkIndex.Y)
                    {
                        ResourceManager.Instance.EntityCache.Unsubscribe(planet, new Index2(Entity.Position.ChunkIndex),
                            (Entity as PermanentEntity).ActivationRange);
                        ResourceManager.Instance.EntityCache.Subscribe(planet, new Index2(position.ChunkIndex),
                            (Entity as PermanentEntity).ActivationRange);
                    }
                }

                // Neue Position setzen
                Entity.Position = position;

                loop++;
            }
            while (collision && loop < 3);



            if (Entity.Position.ChunkIndex != _oldIndex)
            {
                _oldIndex = Entity.Position.ChunkIndex;
                ReadyState = false;
                localChunkCache.SetCenter(planet, new Index2(Entity.Position.ChunkIndex), (success) =>
                {
                    ReadyState = success;
                });
            }

            #endregion
        }

        internal void Unload()
        {
            if (Entity is PermanentEntity)
            {
                ResourceManager.Instance.EntityCache.Unsubscribe(planet, new Index2(Entity.Position.ChunkIndex),
                    (Entity as PermanentEntity).ActivationRange);
            }

            localChunkCache.Flush();
        }

        /// <summary>
        /// Führt die physikalischen Berechnungen für die Entität durch.
        /// TODO: Hier werden noch konstanten aus dem Player verwendet!
        /// </summary>
        /// <param name="velocitydirection">Die Bewegungsrichtung.</param>
        /// <param name="elapsedtime">Die Zeitspanne seit dem letzten PhysicalUpdate.</param>
        /// <param name="gravity">Gibt an, ob Schwerkraft wirken soll.</param>
        /// <param name="jump">Gibt an, ob ein Sprung durchgeführt werden soll.</param>
        /// <returns></returns>
        protected Vector3 PhysicalUpdate(Vector3 velocitydirection, TimeSpan elapsedtime, bool gravity, bool jump)
        {
            Vector3 exforce = gravity ? Entity.ExternalForce : Vector3.Zero;

            if (gravity)
            {
                exforce += ResourceManager.Instance.GetPlanet(Entity.Position.Planet).Gravity * Entity.Mass;
            }

            Vector3 externalPower = ((exforce * exforce) / (2 * Entity.Mass)) * (float)elapsedtime.TotalSeconds;
            externalPower *= new Vector3(Math.Sign(exforce.X), Math.Sign(exforce.Y), Math.Sign(exforce.Z));

            Vector3 friction = new Vector3(1, 1, 0.1f) * Player.FRICTION;
            Vector3 powerdirection = new Vector3();

            powerdirection += externalPower;
            powerdirection += (Player.POWER * velocitydirection);
            if (jump && (Entity.OnGround || !gravity))
            {
                Vector3 jumpDirection = new Vector3(0, 0, 1);
                jumpDirection.Z = 1f;
                jumpDirection.Normalize();
                powerdirection += jumpDirection * Player.JUMPPOWER;
            }

            Vector3 VelocityChange = (2.0f / Entity.Mass * (powerdirection - friction * Entity.Velocity)) *
                (float)elapsedtime.TotalSeconds;

            return new Vector3(
                (float)(VelocityChange.X < 0 ? -Math.Sqrt(-VelocityChange.X) : Math.Sqrt(VelocityChange.X)),
                (float)(VelocityChange.Y < 0 ? -Math.Sqrt(-VelocityChange.Y) : Math.Sqrt(VelocityChange.Y)),
                (float)(VelocityChange.Z < 0 ? -Math.Sqrt(-VelocityChange.Z) : Math.Sqrt(VelocityChange.Z)));

        }
    }
}
