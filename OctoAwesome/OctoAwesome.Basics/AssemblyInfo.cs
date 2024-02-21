using OctoAwesome;
using OctoAwesome.Basics;
using OctoAwesome.Basics.Entities;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Basics.SimulationComponents;
using OctoAwesome.Basics.UI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: NoosonConfiguration(
    GenerateDeserializeExtension = false,
    DisableWarnings = true,
    GenerateStaticDeserializeWithCtor = true,
    GenerateDeserializeOnInstance = true,
    GenerateStaticSerialize = true,
    GenerateStaticDeserializeIntoInstance = true,
    NameOfStaticDeserializeWithCtor = "DeserializeAndCreate",
    NameOfDeserializeOnInstance = "Deserialize",
    NameOfStaticDeserializeIntoInstance = "Deserialize",
    NameOfStaticDeserializeWithOutParams = "DeserializeOut")]

[assembly: SerializationId<WauziEntity>(2, 1)]
[assembly: SerializationId<Chest>(2, 2)]
[assembly: SerializationId<Furnace>(2, 3)]
[assembly: SerializationId<ComplexPlanet>(2, 4)]
[assembly: SerializationId<BodyPowerComponent>(2, 5)]
[assembly: SerializationId<ProductionInventoriesComponent>(2, 6)]
[assembly: SerializationId<BurningComponent>(2, 7)]
[assembly: SerializationId<EntityCollisionComponent>(2, 8)]
[assembly: SerializationId<ForceComponent>(2, 9)]
[assembly: SerializationId<GravityComponent>(2, 10)]
[assembly: SerializationId<MassComponent>(2, 11)]
[assembly: SerializationId<MoveableComponent>(2, 12)]
[assembly: SerializationId<PowerComponent>(2, 13)]
[assembly: SerializationId<RelatedEntityComponent>(2, 14)]
[assembly: SerializationId<TransferComponent>(2, 15)]
[assembly: SerializationId<AccelerationComponent>(2, 16)]
[assembly: SerializationId<BlockInteractionComponent>(2, 17)]
[assembly: SerializationId<CollisionComponent>(2, 18)]
[assembly: SerializationId<ComponentContainerInteractionComponent>(2, 19)]
[assembly: SerializationId<ForceAggregatorComponent>(2, 20)]
[assembly: SerializationId<MoveComponent>(2, 21)]
[assembly: SerializationId<NewtonGravitatorComponent>(2, 22)]
[assembly: SerializationId<PowerAggregatorComponent>(2, 23)]
[assembly: SerializationId<WattMoverComponent>(2, 24)]
[assembly: SerializationId<FurnaceUIComponent>(2, 25)]
[assembly: SerializationId<TransferUIComponent>(2, 26)]
