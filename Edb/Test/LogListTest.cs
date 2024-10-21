using Xunit;

namespace Edb.Test
{
    public class LogListTest
    {
        private XBeanTest xBean;
        
        private void Init()
        {
            var ctx = TransactionCtx.Create().Start();
            _ = ctx.Current!.Savepoint;
            xBean = new XBeanTest(ctx);
            
            var logList = xBean.List;
            Assert.Contains(1, logList);
            Assert.Contains(2, logList);
            Assert.Contains(3, logList);
        }
        
        [Fact]
        public void TestAdd()
        {
            Init();
            
            var logList = xBean.List;
            var originCount = logList.Count;
            logList.Add(4);
            logList.Add(5);
            Assert.Equal(originCount + 2, logList.Count);
            Assert.Contains(4, logList);
            Assert.Contains(5, logList);
        }
        
        [Fact]
        public void TestRemove()
        {
            Init();
            
            var logList = xBean.List;
            var originCount = logList.Count;
            logList.Remove(1);
            logList.Remove(2);
            Assert.Equal(originCount - 2, logList.Count);
            Assert.DoesNotContain(1, logList);
            Assert.DoesNotContain(2, logList);
            Assert.Contains(3, logList);
        }

        [Fact]
        public void TestRemoveAt()
        {
            Init();
            
            var logList = xBean.List;
            var originCount = logList.Count;
            logList.RemoveAt(0);
            logList.RemoveAt(1);
            Assert.Equal(originCount - 2, logList.Count);
        }
        
        [Fact]
        public void TestClear()
        {
            Init();
            
            var logList = xBean.List;
            logList.Clear();
            Assert.Empty(logList);
        }
        
        [Fact]
        public void TestInsert()
        {
            Init();
            
            var logList = xBean.List;
            logList.Insert(0, 0);
            logList.Insert(2, 1);
            logList.Insert(4, 2);
            Assert.Equal(6, logList.Count);
            Assert.Equal(0, logList[0]);
            Assert.Equal(1, logList[2]);
            Assert.Equal(2, logList[4]);
        }
        
        [Fact]
        public void TestIndexer()
        {
            Init();
            
            var logList = xBean.List;
            logList[0] = 0;
            logList[1] = 1;
            logList[2] = 2;
            Assert.Equal(0, logList[0]);
            Assert.Equal(1, logList[1]);
            Assert.Equal(2, logList[2]);
        }

        private class XBeanTest : XBean
        {
            private List<int> m_List = new();
            public XBeanTest(TransactionCtx ctx) : base(null, null!)
            {
                List = Logs.LogList<int>(this, "m_List", DoNothing, ctx);
                List.Add(1);
                List.Add(2);
                List.Add(3);
            }

            public IList<int> List;
        }
    }
}