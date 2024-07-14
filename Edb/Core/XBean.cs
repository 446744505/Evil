namespace Edb
{
    public class XBean
    {
        private static long _objid;

        private long m_ObjId = Interlocked.Increment(ref _objid);
        private XBean m_Parent;
        private string m_VarName;
        
        internal long ObjId => m_ObjId;
        
        protected XBean(XBean parent, string varName)
        {
            m_Parent = parent;
            m_VarName = varName;
        }
    }
}