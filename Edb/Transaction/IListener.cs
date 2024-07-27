namespace Edb
{
    public interface IListener
    {
        public void OnChanged(object key, object val)
        {
        }

        public void OnRemoved(object key, object val)
        {
        }
        
        public void OnChanged(object key, object val, string fullVarName, INote note)
        {
        }
    }
}