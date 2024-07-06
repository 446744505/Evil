namespace Generator.Exception
{
    public class TypeException : System.Exception
    {
        public TypeException(string msg) : base(msg)
        {
            
        }
        
        public TypeException(string msg, System.Exception e) : base(msg, e)
        {
            
        }
    }
}