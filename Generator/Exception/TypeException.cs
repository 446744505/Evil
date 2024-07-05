namespace Generator
{
    public class TypeException : Exception
    {
        public TypeException(string msg) : base(msg)
        {
            
        }
        
        public TypeException(string msg, Exception e) : base(msg, e)
        {
            
        }
    }
}