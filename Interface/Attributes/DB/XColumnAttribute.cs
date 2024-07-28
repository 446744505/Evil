using System;

namespace Attributes
{
    public class XColumnAttribute : Attribute
    {
        private bool m_Id;
        
        public XColumnAttribute()
        {
            m_Id = false;
        }
        
        public XColumnAttribute(bool id)
        {
            m_Id = id;
        }
    }
}