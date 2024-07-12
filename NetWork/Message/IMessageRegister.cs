namespace NetWork
{
    public interface IMessageRegister
    {
        public void Register(IMessageProcessor processor);
    }
}