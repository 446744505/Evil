namespace Edb
{
    public class XBean
    {
        private static long _objId = 0;

        private long m_ObjId = Interlocked.Increment(ref _objId);
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