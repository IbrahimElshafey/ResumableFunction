using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class EventWait : Wait
    {
        public Guid ParentGroupId { get; set; }
        public string EventIdentifier { get; set; }
        public bool IsOptional { get; set; } = false;
        public bool IsFirst { get; set; } = false;
        public LambdaExpression MatchExpression { get; set; }
        public LambdaExpression SetPropExpression { get; set; }

        /// <summary>
        /// Resumable function or subfunction that request the event waiting.
        /// </summary>
        public string InitiatedByFunction { get; set; }
        public string EventProviderName { get; set; }

        /// <summary>
        /// Used by engine to desrialze response to this type
        /// </summary>
        public Type EventDataType { get; set; }

        public dynamic EventData { get; set; }

        private static Delegate? _matchExpressionCompiled;
        public bool IsMatch()
        {
            if (_matchExpressionCompiled is null)
                _matchExpressionCompiled = MatchExpression.Compile();
            return (bool)_matchExpressionCompiled.DynamicInvoke(ParentFunctionState?.Data, EventData);
        }

        private static Delegate? _setPropExpressionCompiled;
        public void SetDataProp()
        {
            if (_setPropExpressionCompiled is null)
                _setPropExpressionCompiled = SetPropExpression.Compile();
            _setPropExpressionCompiled.DynamicInvoke(ParentFunctionState?.Data, EventData);
        }
    }

    public sealed class EventWait<Data> : EventWait where Data : class, IEventData, new()
    {
        public EventWait(string eventIdentifier = "", [CallerMemberName] string callerName = "")
        {
            var instance = new Data();
            EventIdentifier = eventIdentifier ?? instance.EventIdentifier;
            if (EventIdentifier == null)
                throw new NullReferenceException("EventIdentifier can't be null.");
            EventProviderName = instance.EventProviderName;
            if (EventProviderName == null)
                throw new NullReferenceException("EventProviderName can't be null.");
            EventData = instance;
            EventDataType = instance.GetType();
            InitiatedByFunction = callerName;
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
