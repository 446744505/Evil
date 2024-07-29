using Attributes;

namespace Hero
{
    [Protocol(Node.Client | Node.Game)]
    [XBean(Node.Game)]
    public class Properties
    {
        [ProtocolField(1)]
        [XColumn]
        private int abs;
        
        [ProtocolField(2)]
        [XColumn]
        private int pct;
    }
}