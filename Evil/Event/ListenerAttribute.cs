namespace Evil.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ListenerAttribute : Attribute
    {
        private readonly List<Type> m_Events;
        
        public List<Type> Events => m_Events;

        public ListenerAttribute(Type evt)
        {
            m_Events = new List<Type>{evt};
        }
        public ListenerAttribute(Type evt1,Type evt2)
        {
            m_Events = new List<Type>{evt1, evt2};
        }
        public ListenerAttribute(Type evt1,Type evt2, Type evt3)
        {
            m_Events = new List<Type>{evt1, evt2, evt3};
        }
        public ListenerAttribute(Type evt1,Type evt2, Type evt3, Type evt4)
        {
            m_Events = new List<Type>{evt1, evt2, evt3, evt4};
        }
        public ListenerAttribute(params Type[] events)
        {
            m_Events = new List<Type>(events);
        }
    }
}