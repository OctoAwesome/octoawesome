﻿using OctoAwesome.Basics.Entities;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basics.SimulationComponents;
using OctoAwesome.EntityComponents;
using System.Reflection;
using System;
using engenious;
using OctoAwesome.Services;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Basics.EntityComponents.UIComponents;
using OctoAwesome.Extension;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Extension implementation for this library(<see cref="OctoAwesome.Basics"/>).
    /// </summary>
    public sealed class Extension : IExtension
    {
        /// <inheritdoc />
        public string Description => Languages.OctoBasics.ExtensionDescription;

        /// <inheritdoc />
        public string Name => Languages.OctoBasics.ExtensionName;


        /// <inheritdoc />
        public void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<IPlanet, ComplexPlanet>();
        }

        /// <inheritdoc />
        public void Register(ExtensionService extensionLoader)
        {

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!t.IsAbstract && typeof(IDefinition).IsAssignableFrom(t))
                    extensionLoader.Register(t);
            }

            extensionLoader.Register(new ComplexPlanetGenerator());

            extensionLoader.Register(new TreePopulator());
            extensionLoader.Register(new WauziPopulator());

            extensionLoader.Register(typeof(WauziEntity), "Serialization");
            extensionLoader.Register(typeof(Chest), "Serialization");

            extensionLoader.Extend<WauziEntity>(wauziEntity => wauziEntity.RegisterDefault());

            extensionLoader.Extend<Player>((player) =>
            {
                var p = (Player)player;
                var posComponent = new PositionComponent { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0)) };

                p.Components.AddComponent(posComponent);
                p.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 3.5f, Radius = 0.75f });
                p.Components.AddComponent(new BodyPowerComponent() { Power = 600f, JumpTime = 120 });
                p.Components.AddComponent(new GravityComponent());
                p.Components.AddComponent(new MoveableComponent());
                p.Components.AddComponent(new BoxCollisionComponent(Array.Empty<BoundingBox>()));
                p.Components.AddComponent(new EntityCollisionComponent());
                p.Components.AddComponent(new LocalChunkCacheComponent(posComponent.Planet.GlobalChunkCache, 4, 2));
                p.Components.AddComponent(new TransferComponent());
            });

            extensionLoader.Extend<Chest>((chest) =>
            {
                var c = chest;

                if (c is null)
                    return;

                if (!c.ContainsComponent<PositionComponent>())
                {
                    var pos = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0));
                    c.Components.AddComponent(new PositionComponent()
                    {
                        Position = pos
                    });

                }

                if (!c.Components.TryGetComponent<AnimationComponent>(out var animationComponent))
                {
                    c.animationComponent = new AnimationComponent();
                    c.Components.AddComponent(c.animationComponent);
                }
                else
                    c.animationComponent = animationComponent;

                if (!c.Components.TryGetComponent<InventoryComponent>(out var inventoryComponent))
                {
                    inventoryComponent = new InventoryComponent();
                    c.Components.AddComponent(inventoryComponent);
                }

                if (!c.ContainsComponent<TransferUIComponent>())
                {
                    c.transferUiComponent = new TransferUIComponent(inventoryComponent);
                    c.transferUiComponent.Closed += c.TransferUiComponentClosed;
                    c.Components.AddComponent(c.transferUiComponent, true);
                }


                c.Components.AddComponent(new BodyComponent() { Height = 0.4f, Radius = 0.2f }, true);
                c.Components.AddComponent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0), new Vector3(1, 1, 1)) }), true);
                c.Components.AddComponent(new RenderComponent() { Name = "Chest", ModelName = "chest", TextureName = "texchestmodel", BaseZRotation = -90 }, true);

            });


            extensionLoader.Extend<Simulation>((s) =>
            {
                s.Components.AddComponent(new WattMoverComponent());
                s.Components.AddComponent(new NewtonGravitatorComponent());
                s.Components.AddComponent(new ForceAggregatorComponent());
                s.Components.AddComponent(new PowerAggregatorComponent());
                s.Components.AddComponent(new AccelerationComponent());
                s.Components.AddComponent(new MoveComponent());
                s.Components.AddComponent(new BlockInteractionComponent(s, typeContainer.Get<BlockCollectionService>()));

                //TODO: ugly
                //TODO: TypeContainer?
            });
        }
    }
}
