using System;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class XColumnAttribute : Attribute
    {
        private readonly bool m_Id;
        
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