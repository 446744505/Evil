namespace Evil.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ListenerAttribute : Attribute
    {
        private readonly List<Type> m_Events;
        
        public List<Type> Events => m_Events;

        public ListenerAttribute(params Type[] events)
        {
            m_Events = new List<Type>(events);
        }
    }
}