
using System.Reflection;
using Edb;
using Evil.Util;

namespace Evil.Event
{
    public class Event
    {
        private static readonly Dictionary<Type, List<Action<IEvent>>> Listeners = new();

        public static void Start()
        {
            // 先放这里吧，如果有其他系统也用到Search功能，则挪到外面去避免查找多次
            AssemblySearch.DoSearch(new EventSearchAble());
        }

        public static void Fire(IEvent e)
        {
            var type = e.GetType();
            if (Listeners.TryGetValue(type, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    try
                    {
                        listener(e);
                    } catch (Exception ex)
                    {
                        Log.I.Error($"Event {type} listener error", ex);
                    }
                }
            }
        }

        public static void FireWhenCommit(IEvent e)
        {
            Transaction.AddSavepointTask(() => Fire(e), null);
        }
        
        public static void Register<T>(Action<T> listener) where T : IEvent
        {
            var type = typeof(T);
            if (!Listeners.TryGetValue(type, out var listeners))
            {
                listeners = new List<Action<IEvent>>();
                Listeners.Add(type, listeners);
            }
            listeners.Add(e => listener((T)e));
        }

        public static void Register(Type type, Action<IEvent> listener)
        {
            if (!Listeners.TryGetValue(type, out var listeners))
            {
                listeners = new List<Action<IEvent>>();
                Listeners.Add(type, listeners);
            }
            listeners.Add(listener);
        }
    }

    public interface IEvent
    {
    }

    /// <summary>
    /// 用作反射查找识别
    /// </summary>
    public interface IEventHandler
    {
    }

    public class EventSearchAble : ISearchAble
    {
        private static readonly ThreadLocal<Dictionary<Type, object>> Objs = new(() => new Dictionary<Type, object>());
        public bool IsSearched(Type type)
        {
            return typeof(IEventHandler).IsAssignableFrom(type);
        }

        public void OnSearch(List<Type> types)
        {
            // 找到所有有Listener特性的方法
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods())
                {
                    var attr = method.GetCustomAttribute<ListenerAttribute>();
                    if (attr == null) 
                        continue;
                    var isStatic = method.IsStatic;
                    foreach (var eventType in attr.Events)
                    {
                        Event.Register(eventType, evt =>
                        {
                            var param = method.GetParameters();
                            var obj = isStatic ? null : GetOrCreateInstance(type);
                            method.Invoke(obj, param.Length == 0 ? null : new object?[]{evt});
                        });
                    }
                }
            }
        }

        private object GetOrCreateInstance(Type type)
        {
            if (Objs.Value!.TryGetValue(type, out var obj))
            {
                return obj;
            }
            obj = Activator.CreateInstance(type)!;
            Objs.Value[type] = obj;
            return obj;
        }
    }
}