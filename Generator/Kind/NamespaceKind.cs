namespace Generator;

public class NamespaceKind : BaseKind
{
    private readonly string m_Name;
    
    public NamespaceKind(string name) : base(null!)
    {
        m_Name = name;
    }
}