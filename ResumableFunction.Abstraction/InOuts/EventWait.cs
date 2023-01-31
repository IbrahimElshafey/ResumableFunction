using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class EventWait : Wait
    {
        public Guid? ParentFunctionId { get; internal set;}
        public Guid? ParentGroupId { get; internal set;}
        public bool IsOptional { get; internal set;} = false;
        
        public LambdaExpression MatchExpression { get; internal set;}
        public LambdaExpression SetDataExpression { get; internal set;}

        /// <summary>
        /// Resumable function or subfunction that request the event waiting.
        /// </summary>
        
        public string EventProviderName { get; internal set;}

        /// <summary>
        /// Used by engine to desrialze response to this type
        /// </summary>
        public Type EventDataType { get; internal set;}

        public dynamic EventData { get; internal set;}
        public bool NeedFunctionData { get; internal set; }

        private static Delegate? _matchExpressionCompiled;
        public bool IsMatch()
        {
            if (_matchExpressionCompiled is null)
                _matchExpressionCompiled = MatchExpression.Compile();
            return (bool)_matchExpressionCompiled.DynamicInvoke(FunctionRuntimeInfo?.Data, EventData);
        }

        private static Delegate? _setPropExpressionCompiled;
        public void SetData()
        {
            if (_setPropExpressionCompiled is null)
                _setPropExpressionCompiled = SetDataExpression.Compile();
            _setPropExpressionCompiled.DynamicInvoke(FunctionRuntimeInfo?.Data, EventData);
        }
    }

    public sealed class EventWait<EventData> : EventWait where EventData : class, IEventData, new()
    {
        public EventWait(string eventIdentifier = "", [CallerMemberName] string callerName = "")
        {
            var instance = new EventData();
            EventIdentifier = eventIdentifier ?? instance.EventIdentifier;
            if (EventIdentifier == null)
                throw new NullReferenceException("EventIdentifier can't be null.");
            EventProviderName = instance.EventProviderName;
            if (EventProviderName == null)
                throw new NullReferenceException("EventProviderName can't be null.");
            base.EventData = instance;
            EventDataType = instance.GetType();
            InitiatedByFunction = callerName;
        }



        public EventWait<EventData> Match(Expression<Func<EventData, bool>> func)
        {
            MatchExpression = func;
            return this;
        }

        public EventWait<EventData> SetData(Expression<Func<EventData>> instancePropFunc)
        {
            SetDataExpression = instancePropFunc;
            return this;
        }

        public EventWait<EventData> SetData(Expression<Func<List<EventData>>> instancePropFunc)
        {
            SetDataExpression = instancePropFunc;
            return this;
        }


        public EventWait<EventData> SetOptional()
        {
            IsOptional = true;
            return this;
        }

    }



}
