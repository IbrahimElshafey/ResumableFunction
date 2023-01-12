using System.Linq.Expressions;

namespace WorkflowInCode.Abstraction.InOuts
{
    /// <summary>
    /// This type saved to databse after waiting for and event
    /// </summary>
    public class EventWaiting
    {
        public EventWaiting()
        {

        }
        public EventWaiting(IEvent eventToWait)
        {
            if (eventToWait == null) throw new NullReferenceException("eventToWait param in EventWaiting..ctor");
            EventData = eventToWait;
            EventProviderName = eventToWait.EventProviderName;
            EventType = eventToWait.GetType().FullName;
        }

        public bool IsOptional { get; private set; } = false;
        public LambdaExpression MatchExpression { get; private set; }
        public LambdaExpression SetPropExpression { get; private set; }

        public object EventData { get; set; }
        public string EventProviderName { get; set; }
        public string EventType { get; set; }

        public EventWaiting Match<T>(Expression<Func<T, bool>> func) where T : IEvent
        {
            MatchExpression = func;
            return this;
        }

        public EventWaiting SetProp<T>(Expression<Func<T>> instancePropFunc) where T : IEvent
        {
            SetPropExpression = instancePropFunc;
            return this;
        }

        public EventWaiting SetOptional()
        {
            IsOptional = true;
            return this;
        }
    }



}
