namespace OctoAwesome.Definitions.Items
{

    public class HandDefinition : IItemDefinition
    {
        public string Name => nameof(Hand);
        public string Icon => "";

        private readonly Hand hand;
        public HandDefinition()
        {
            hand = new Hand(this);
        }
        public bool CanMineMaterial(IMaterialDefinition material) 
            => true;
        public Item Create(IMaterialDefinition material)
            => hand;
    }
}
