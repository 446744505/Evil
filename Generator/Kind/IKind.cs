namespace Generator;

public interface IKind
{
    public IKind Parent();
    public void AddChild(IKind child);
}