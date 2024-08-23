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
        
        public static void ExecuteWhenRollback(Action action)
        {
            Transaction.AddSavepointTask(null, action);
        }
        
        public static void ExecuteWhenFinish(Action action)
        {
            Transaction.AddSavepointTask(action, action);
        }
        
        public class MessageDispatcher : IMessageDispatcher
        {
            public async Task<bool> Dispatch(Message msg)
            {
                var result = await Procedure.Submit(msg.Process);
                return result.IsSuccess;
            }
        }
    }
}