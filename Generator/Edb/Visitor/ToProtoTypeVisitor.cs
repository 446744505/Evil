using System;
using Generator.Kind;
using Generator.Type;
using Generator.Util;

namespace Generator.Visitor
{
    public class ToProtoTypeVisitor : ITypeVisitor
    {
        private readonly XBeanFieldKind m_FieldKind;
        private readonly Writer m_Writer;
        private readonly int m_InitTabN;
        private readonly string m_RightValueName; // 不为空则表示只生成右值

        private string FieldName => m_FieldKind.Name;
        private string RightValueResult { get; set; }
        private bool IsRightValue => !string.IsNullOrEmpty(m_RightValueName);

        private ToProtoTypeVisitor(string rightValueName)
        {
            m_RightValueName = rightValueName;
        }
        
        public ToProtoTypeVisitor(XBeanFieldKind fieldKind, Writer writer, int initTabN)
        {
            m_FieldKind = fieldKind;
            m_Writer = writer;
            m_InitTabN = initTabN;
        }
        
        private void RefVisit()
        {
            if (IsRightValue)
            {
                RightValueResult = $"{m_RightValueName}.ToProto()";
                return;
            }
            m_Writer.WriteLine(m_InitTabN,$"_p_.{FieldName} = {FieldName}.ToProto();");
        }
        
        public void Visit(StructType type)
        {
            RefVisit();
        }

        public void Visit(ClassType type)
        {
            RefVisit();
        }

        private void BaseVisit()
        {
            if (IsRightValue)
            {
                RightValueResult = m_RightValueName;
                return;
            }
            m_Writer.WriteLine(m_InitTabN,$"_p_.{FieldName} = {FieldName};");
        }
        
        public void Visit(IntType type)
        {
            BaseVisit();
        }

        public void Visit(LongType type)
        {
            BaseVisit();
        }

        public void Visit(BoolType type)
        {
            BaseVisit();
        }

        public void Visit(StringType type)
        {
            BaseVisit();
        }

        public void Visit(FloatType type)
        {
            BaseVisit();
        }

        public void Visit(DoubleType type)
        {
            BaseVisit();
        }

        public void Visit(ListType type)
        {
            if (IsRightValue)
                throw new NotSupportedException();
            
            var rightValueVisitor = new ToProtoTypeVisitor("_v_");
            type.Value().Accept(rightValueVisitor);
            m_Writer.WriteLine(m_InitTabN,$"foreach (var _v_ in {FieldName})");
            m_Writer.WriteLine(m_InitTabN,"{");
            m_Writer.WriteLine(m_InitTabN + 1,$"_p_.{FieldName}.Add({rightValueVisitor.RightValueResult});");
            m_Writer.WriteLine(m_InitTabN,"}");
        }

        public void Visit(MapType type)
        {
            if (IsRightValue)
                throw new NotSupportedException();
            
            var rightValueVisitor = new ToProtoTypeVisitor("_pair_.Value");
            type.Value().Accept(rightValueVisitor);
            m_Writer.WriteLine(m_InitTabN,$"foreach (var _pair_ in {FieldName})");
            m_Writer.WriteLine(m_InitTabN,"{");
            m_Writer.WriteLine(m_InitTabN + 1,$"_p_.{FieldName}.Add(_pair_.Key, {rightValueVisitor.RightValueResult});");
            m_Writer.WriteLine(m_InitTabN,"}");
        }

        public void Visit(TaskType type)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(WaitCompileIdentiferType type)
        {
            throw new System.NotImplementedException();
        }
    }
}