using OctoAwesome;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;

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

[assembly: SerializationId<Player>(1, 1)]
[assembly: SerializationId<BlocksChangedNotification>(1, 2)]
[assembly: SerializationId<BlockChangedNotification>(1, 3)]
[assembly: SerializationId<EntityNotification>(1, 4)]
[assembly: SerializationId<Planet>(1, 5)]
[assembly: SerializationId<AnimationComponent>(1, 6)]
[assembly: SerializationId<BodyComponent>(1, 7)]
[assembly: SerializationId<BoxCollisionComponent>(1, 8)]
[assembly: SerializationId<ControllableComponent>(1, 9)]
[assembly: SerializationId<HeadComponent>(1, 10)]
[assembly: SerializationId<InteractKeyComponent>(1, 11)]
[assembly: SerializationId<InventoryComponent>(1, 12)]
[assembly: SerializationId<LocalChunkCacheComponent>(1, 13)]
[assembly: SerializationId<PositionComponent>(1, 14)]
[assembly: SerializationId<RenderComponent>(1, 15)]
[assembly: SerializationId<ServerManagedComponent>(1, 16)]
[assembly: SerializationId<UniquePositionComponent>(1, 17)]
[assembly: SerializationId<ToolBarComponent>(1, 18)]
[assembly: SerializationId<ChatNotification>(1, 19)]