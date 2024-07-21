using Evil.Util;
using Xunit;

namespace Edb.Test
{
    public class LogMapTest
    {
        private XBeanTest xBean;
        
        private void Init()
        {
            Transaction.Create();
            Transaction.Savepoint();
            xBean = new XBeanTest();
            
            var logMap = xBean.Map;
            Assert.Contains(1, logMap);
            Assert.Contains(2, logMap);
            Assert.Contains(3, logMap);
        }
        
        [Fact]
        public void TestAdd()
        {
            Init();
            
            var logMap = xBean.Map;
            var originCount = logMap.Count;
            logMap.Add(4, 4);
            logMap.Add(5, 5);
            try
            {
                logMap.Add(4, 4);
            } catch (ArgumentException e)
            {
                Assert.Contains("already exists in dictionary", e.Message);
            }
            Assert.Equal(originCount + 2, logMap.Count);
            Assert.Contains(4, logMap);
            Assert.Contains(5, logMap);
        }
        
        [Fact]
        public void TestRemove()
        {
            Init();
            
            var logMap = xBean.Map;
            var originCount = logMap.Count;
            logMap.Remove(1);
            logMap.Remove(2);
            Assert.False(logMap.Remove(1));
            Assert.Equal(originCount - 2, logMap.Count);
            Assert.DoesNotContain(1, logMap);
            Assert.DoesNotContain(2, logMap);
        }
        
        [Fact]
        public void TestClear()
        {
            Init();
            
            var logMap = xBean.Map;
            logMap.Clear();
            Assert.Empty(logMap);
        }

        [Fact]
        public void TestRemoveAndReturnValue()
        {
            Init();
            
            var logMap = xBean.Map;
            var originCount = logMap.Count;
            var success = logMap.Remove(1, out var value);
            Assert.True(success);
            Assert.Equal(1, value);
            Assert.False(logMap.Remove(1, out value));
            Assert.Equal(originCount - 1, logMap.Count);
            Assert.DoesNotContain(1, logMap);
        }
        
        [Fact]
        public void TestPutAndReturnValue()
        {
            Init();
            
            var logMap = xBean.Map;
            var originCount = logMap.Count;
            var success = logMap.PutAndReturnValue(1, 4, out var origin);
            Assert.True(success);
            Assert.Equal(1, origin);
            Assert.Equal(originCount, logMap.Count);
            Assert.Equal(4, logMap[1]);
        }
        
        [Fact]
        public void TestIndexer()
        {
            Init();
            
            var logMap = xBean.Map;
            Assert.Equal(1, logMap[1]);
            Assert.Equal(2, logMap[2]);
            Assert.Equal(3, logMap[3]);
            logMap[1] = 4;
            logMap[2] = 5;
            logMap[3] = 6;
            Assert.Equal(4, logMap[1]);
            Assert.Equal(5, logMap[2]);
            Assert.Equal(6, logMap[3]);
        }
        
        private class XBeanTest : XBean
        {
            private Dictionary<int, int> m_Map = new();
            public XBeanTest() : base(null, null)
            {
                Map.Add(1, 1);
                Map.Add(2, 2);
                Map.Add(3, 3);
            }
            
            public IDictionary<int, int> Map => Logs.LogMap<int, int>(this, "m_Map", DoNothing);
        }
    }
}