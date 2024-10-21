using Edb;
using NetWork;

namespace Evil.Util
{
    public class ProcedureHelper
    {
        public static void SendWhenCommit(Session session, Message msg, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(() =>
            {
                msg.Send(session);
            }, null, ctx);
        }
        
        public static void SendWhenRollback(Session session, Message msg, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(null, () =>
            {
                msg.Send(session);
            }, ctx);
        }
        
        public static void SendWhenFinish(Session session, Message msg, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(() =>
            {
                msg.Send(session);
            }, () =>
            {
                msg.Send(session);
            }, ctx);
        }
        
        public static void ExecuteWhenCommit(Action action, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(action, null, ctx);
        }
        
        public static void ExecuteWhenCommit(Procedure p, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(p, null, ctx);
        }
        
        public static void ExecuteWhenRollback(Action action, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(null, action, ctx);
        }
        
        public static void ExecuteWhenRollback(Procedure p, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(null, p, ctx);
        }
        
        public static void ExecuteWhenFinish(Action action, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(action, action, ctx);
        }
        
        public static void ExecuteWhenFinish(Procedure p, TransactionCtx ctx)
        {
            Transaction.AddSavepointTask(p, p, ctx);
        }
        
        public class MessageDispatcher : IMessageDispatcher
        {
            public Task Dispatch(Message msg)
            {
                return Procedure.Submit(msg.Process);
            }
        }
    }
}