namespace Edb
{
    public interface IListener
    {
        public Task OnChanged(object key, object val)
        {
            return Task.CompletedTask;
        }

        public Task OnRemoved(object key, object val)
        {
            return Task.CompletedTask;
        }
        
        public Task OnChanged(object key, object val, string fullVarName, INote? note)
        {
            return Task.CompletedTask;
        }
    }
}