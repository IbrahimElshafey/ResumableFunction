using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace WorkflowInCode.Abstraction.InOuts
{

    /// <summary>
    /// This type saved to databse after waiting for and event
    /// </summary>
    public class SingleEventWaiting : EventWaitingResult
    {
        public SingleEventWaiting()
        {

        }
        public SingleEventWaiting(Type eventType, string eventName, [CallerMemberName] string callerName = "")
        {
            if (eventType == null) throw new NullReferenceException("eventToWait param in EventWaiting..ctor");
            var instance = (IEventData)Activator.CreateInstance(eventType);
            if (instance is null)
            {
                throw new NullReferenceException($"Can't initiate instance of {eventType.FullName} with constructor less parameters.");
            }
            EventName = eventName;
            EventData = instance;
            EventProviderName = instance.EventProviderName;
            EventType = eventType;
            InitiatedByMethod = callerName;
        }
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public bool IsOptional { get; private set; } = false;
        public LambdaExpression MatchExpression { get; private set; }
        public LambdaExpression SetPropExpression { get; private set; }
        public string InitiatedByMethod { get; set; }
        public Type InitiatedByType { get; set; }

        public IEventData EventData { get; set; }
        public string EventProviderName { get; set; }
        public Type EventType { get; set; }
        public Type WorkflowInstanceDataType { get; set; }
        public Guid WorkflowInstanceId { get; set; }

        public SingleEventWaiting Match<T>(Expression<Func<T, bool>> func) where T : IEventData
        {
            MatchExpression = func;
            return this;
        }

        public SingleEventWaiting SetProp<T>(Expression<Func<T>> instancePropFunc) where T : IEventData
        {
            SetPropExpression = instancePropFunc;
            return this;
        }

        public SingleEventWaiting SetOptional()
        {
            IsOptional = true;
            return this;
        }
    }



}
