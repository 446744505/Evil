namespace NetWork
{
    public class Message
    {
        public virtual uint MessageId { get; set; }
        public Session Session { get; set; }
    }
}