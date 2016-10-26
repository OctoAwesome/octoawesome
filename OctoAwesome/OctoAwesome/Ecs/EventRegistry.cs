namespace OctoAwesome.Ecs
{
    public class EventRegistry<TEvent> where TEvent : GameEvent
    {
        // ReSharper disable once StaticMemberInGenericType
        public static int Index;

        public static void Initialize(int index)
        {
            Index = index;
        }
    }
}