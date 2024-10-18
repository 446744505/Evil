using Edb;
using NetWork;

namespace Evil.Util
{
    public class ProcedureHelper
    {
        public static void SendWhenCommit(Session session, Message msg)
        {
            Transaction.AddSavepointTask(() =>
            {
                msg.Send(session);
            }, null);
        }
        
        public static void SendWhenRollback(Session session, Message msg)
        {
            Transaction.AddSavepointTask(null, () =>
            {
                msg.Send(session);
            });
        }
        
        public static void SendWhenFinish(Session session, Message msg)
        {
            Transaction.AddSavepointTask(() =>
            {
                msg.Send(session);
            }, () =>
            {
                msg.Send(session);
            });
        }
        
        public static void ExecuteWhenCommit(Action action)
        {
            Transaction.AddSavepointTask(action, null);
        }
        
        public static void ExecuteWhenCommit(Procedure p)
        {
            Transaction.AddSavepointTask(p, null);
        }
        
        public static void ExecuteWhenRollback(Action action)
        {
            Transaction.AddSavepointTask(null, action);
        }
        
        public static void ExecuteWhenRollback(Procedure p)
        {
            Transaction.AddSavepointTask(null, p);
        }
        
        public static void ExecuteWhenFinish(Action action)
        {
            Transaction.AddSavepointTask(action, action);
        }
        
        public static void ExecuteWhenFinish(Procedure p)
        {
            Transaction.AddSavepointTask(p, p);
        }
        
        public class MessageDispatcher : IMessageDispatcher
        {
            public void Dispatch(Message msg)
            {
                Procedure.Execute(msg.Process);
            }
        }
    }
}