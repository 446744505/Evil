using Xunit;

namespace Edb.Test
{
    public class LogSetTest
    {
        private XBeanTest xBean;
        
        private void Init()
        {
            Transaction.Create();
            Transaction.Savepoint();
            xBean = new XBeanTest();
            
            var logSet = xBean.Set;
            Assert.Contains(1, logSet);
            Assert.Contains(2, logSet);
            Assert.Contains(3, logSet);
        }
        
        [Fact]
        public void TestAdd()
        {
            Init();
            
            var logSet = xBean.Set;
            var originCount = logSet.Count;
            Assert.True(logSet.Add(4));
            Assert.True(logSet.Add(5));
            Assert.False(logSet.Add(5));
            Assert.Equal(originCount + 2, logSet.Count);
            Assert.Contains(4, logSet);
            Assert.Contains(5, logSet);
        }
        
        [Fact]
        public void TestRemove()
        {
            Init();
            
            var logSet = xBean.Set;
            var originCount = logSet.Count;
            Assert.True(logSet.Remove(1));
            Assert.True(logSet.Remove(2));
            Assert.False(logSet.Remove(2));
            Assert.Equal(originCount - 2, logSet.Count);
            Assert.DoesNotContain(1, logSet);
            Assert.DoesNotContain(2, logSet);
            Assert.Contains(3, logSet);
        }
        
        [Fact]
        public void TestClear()
        {
            Init();
            
            var logSet = xBean.Set;
            logSet.Clear();
            Assert.Empty(logSet);
        }


        private class XBeanTest : XBean
        {
            private HashSet<int> m_Set = new();
            public XBeanTest() : base(null, null)
            {
                Set.Add(1);
                Set.Add(2);
                Set.Add(3);
            }
            
            public ISet<int> Set => Logs.LogSet<int>(this, "m_Set", DoNothing);
        }
    }
}