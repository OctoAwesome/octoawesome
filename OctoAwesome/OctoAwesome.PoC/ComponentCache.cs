using OctoAwesome.EntityComponents;

using System;

namespace OctoAwesome.PoC
{

    public class ComponentCache : Cache<int, Component>
    {
        protected override Component Load(int key)
        {
            throw new NotImplementedException();
        }
    }

    public class EntityCache : Cache<int, Entity>//, IKeyFinder<Index3>
    {

        //All Position Components  of Entites

        protected Entity Load(Index3 key)
        {
            throw new NotImplementedException();
        }

        protected override Entity Load(int key)
        {
            throw new NotImplementedException();
        }

        
    }

    public class PositionComponentCache : ComponentCache//, IKeyFinder<Index3, PositionComponent>
    {
        protected PositionComponent[] Find(Index3 key)
        {
            //Search Index3 to get Entity ID
            //var pos = PositionComponent.FirstOrDefault(x=>x == key);
            //return (PositionComponent)Load(0 /*pos.EntityId*/);
            throw new NotImplementedException();

        }
    }


    public class Index3PositionConverter //IKeyConverter<Index3, PositionComponent>, IKeyConverter<Index2, PositionComponent>
    {
        protected PositionComponent Convert(Index3 key)
        {
            throw new NotImplementedException();

        }
        protected PositionComponent Convert(Index2 key)
        {
            throw new NotImplementedException();

        }
    }


    public class ChunkColumnCache : Cache<Index3, ChunkColumn>
    {
        protected override ChunkColumn Load(Index3 key)
        {
            throw new NotImplementedException();
        }
    }
}
