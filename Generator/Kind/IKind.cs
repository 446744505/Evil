namespace Generator.Kind
{
    public interface IKind
    {
        public IKind Parent();
        public void AddChild(IKind child);
        public List<IKind> Children();
        void Compile(CompileContext ctx);
    }
}