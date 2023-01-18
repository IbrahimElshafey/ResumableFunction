using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ResumableFunction.Abstraction.InOuts
{

    /// <summary>
    /// This type saved to databse after waiting for and event
    /// </summary>
    public class SingleEventWaiting : EventWaitingResult
    {
        public SingleEventWaiting(Type eventType, string eventName, [CallerMemberName] string callerName = "")
        {
            if (eventType == null) throw new NullReferenceException("eventToWait param in EventWaiting..ctor");
            var instance = (IEventData)Activator.CreateInstance(eventType);
            if (instance is null)
            {
                throw new NullReferenceException($"Can't initiate instance of {eventType.FullName} with constructor less parameters.");
            }
            EventName = eventName;
            //EventData = instance;
            EventProviderName = instance.EventProviderName;
            EventType = eventType;
            InitiatedByFunction = callerName;
        }
        
        public Guid ParentGroupId { get; set; }
        public string EventName { get; set; }
        public bool IsOptional { get; set; } = false;
        public LambdaExpression MatchExpression { get; set; }
        public LambdaExpression SetPropExpression { get; set; }

        /// <summary>
        /// Resumable function that request the event waiting.
        /// </summary>
        public string InitiatedByFunction { get; set; }

        /// <summary>
        /// The class that contians the resumable functions
        /// </summary>
        public Type InitiatedByClass { get; set; }

        public string EventProviderName { get; set; }
        public Type EventType { get; set; }
        public Type FunctionDataType { get; set; }
        public Guid FunctionId { get; set; }

        public SingleEventWaiting Match<T>(Expression<Func<T, bool>> func) where T : IEventData
        {
            //todo:rerwite expression and replace every FunctionData.Prop with constant value
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

        private static Func<IEventData, bool> _matchExpression;
        public bool IsMatch(IEventData eventData)
        {
            if(_matchExpression == null)
                _matchExpression = (Func<IEventData, bool>) MatchExpression.Compile();
            return _matchExpression(eventData);
        }
    }



}
