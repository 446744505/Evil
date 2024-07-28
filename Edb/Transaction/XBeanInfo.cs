using System.Reflection;

namespace Edb
{
    public class XBeanInfo
    {
        private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> m_Infoes = new();

        private static Dictionary<string, FieldInfo> GetFieldsMap(XBean xBean)
        {
            var xBeanType = xBean.GetType();
            if (m_Infoes.TryGetValue(xBeanType, out var fieldMap)) 
                return fieldMap;
            
            fieldMap = new Dictionary<string, FieldInfo>();
            foreach (var field in xBeanType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                fieldMap[field.Name] = field;
            }
            m_Infoes[xBeanType] = fieldMap;
            return fieldMap;
        }

        internal static object? GetValue(XBean xBean, string varName)
        {
            try
            {
                return GetFieldsMap(xBean)[varName].GetValue(xBean);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        internal static void SetValue(XBean xBean, string varName, object? value)
        {
            GetFieldsMap(xBean)[varName].SetValue(xBean, value);
        }
        
        internal static ICollection<FieldInfo> GetFields(XBean xBean)
        {
            return GetFieldsMap(xBean).Values;
        }
    }
}