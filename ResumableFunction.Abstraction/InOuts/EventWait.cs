using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class EventWait : Wait
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
        public bool NeedFunctionDataForMatch { get; set; }

        private static Delegate? _matchExpressionCompiled;
        public bool IsMatch(object functiondata, object eventData)
        {
            if (_matchExpressionCompiled is null)
                _matchExpressionCompiled = MatchExpression.Compile();
            return (bool)_matchExpressionCompiled.DynamicInvoke(functiondata, eventData);
        }
    }

    public sealed class EventWait<Data> : EventWait where Data : class, IEventData, new()
    {
        public Data EventData { get; set; }

        public EventWait(string eventIdentifier = "", [CallerMemberName] string callerName = "")
        {
            var instance = new Data();
            EventIdentifier = eventIdentifier ?? instance.EventIdentifier;
            if (EventIdentifier == null)
                throw new NullReferenceException("EventIdentifier can't be null.");
            this.EventData = instance;
            EventProviderName = instance.EventProviderName;
            if (EventProviderName == null)
                throw new NullReferenceException("EventProviderName can't be null.");
            EventType = instance.GetType();
            InitiatedByFunction = callerName;
        }

        public EventWait<Data> Match<FunctionData>(Expression<Func<FunctionData, Data, bool>> func)
        {
            MatchExpression = func;
            NeedFunctionDataForMatch = true;
            return this;
        }

        public EventWait<Data> Match(Expression<Func<Data, bool>> func)
        {
            MatchExpression = func;
            return this;
        }

        public EventWait<Data> SetProp(Expression<Func<Data>> instancePropFunc)
        {
            SetPropExpression = instancePropFunc;
            return this;
        }

        public EventWait<Data> SetOptional()
        {
            IsOptional = true;
            return this;
        }

    }



}
