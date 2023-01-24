using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class SingleEventWait : EventWaitingResult
    {
        public Guid ParentGroupId { get; set; }
        public string EventIdentifier { get; set; }
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

        /// <summary>
        /// Used by engine to desrialze response to this type
        /// </summary>
        public Type EventType { get; set; }
        public Type FunctionDataType { get; set; }
        public Guid FunctionId { get; set; }

        private static Delegate? _matchExpressionCompiled;
        public bool IsMatch(object eventData)
        {
            if (_matchExpressionCompiled is null)
                _matchExpressionCompiled = MatchExpression.Compile();
            return (bool)_matchExpressionCompiled.DynamicInvoke(eventData);
        }
    }

    public class SingleEventWait<T> : SingleEventWait where T : class, IEventData, new()
    {
        public T EventData { get; set; }

        public SingleEventWait(string eventIdentifier = "", [CallerMemberName] string callerName = "")
        {
            var instance = new T();
            EventIdentifier = eventIdentifier ?? instance.EventIdentifier;
            if (EventIdentifier == null)
                throw new NullReferenceException("EventIdentifier can't be null.");
            EventData = instance;
            EventProviderName = instance.EventProviderName;
            if (EventProviderName == null)
                throw new NullReferenceException("EventProviderName can't be null.");
            EventType = instance.GetType();
            InitiatedByFunction = callerName;
        }


        public SingleEventWait<T> Match(Expression<Func<T, bool>> func)
        {
            MatchExpression = func;
            return this;
        }

        public SingleEventWait<T> SetProp(Expression<Func<T>> instancePropFunc)
        {
            SetPropExpression = instancePropFunc;
            return this;
        }

        public SingleEventWait<T> SetOptional()
        {
            IsOptional = true;
            return this;
        }

    }



}
