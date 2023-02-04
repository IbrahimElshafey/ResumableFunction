using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ResumableFunction.Abstraction.InOuts
{
    public class EventWait : Wait
    {
        public Wait ParentWaitsGroup { get; internal set; }

        [ForeignKey(nameof(ParentWaitsGroup))]
        public int? ParentGroupId { get; internal set; }

        public bool IsOptional { get; internal set; } = false;

        public LambdaExpression MatchExpression { get; internal set; }
        public LambdaExpression SetDataExpression { get; internal set; }



        public string EventProviderName { get; internal set; }

        /// <summary>
        /// Used by engine to desrialze response to this type
        /// </summary>
        public Type EventDataType { get; internal set; }

        public dynamic EventData { get; internal set; }

        //todo:to be used later for enhancements
        public bool NeedFunctionDataForMatch { get; internal set; }

        public bool IsMatch()
        {
            var matchExpressionCompiled = MatchExpression.Compile();
            try
            {
                return (bool)matchExpressionCompiled.DynamicInvoke(CurrntFunction, EventData);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UpdateFunctionData()
        {
            var setPropExpressionCompiled = SetDataExpression.Compile();
            try
            {
                setPropExpressionCompiled.DynamicInvoke(CurrntFunction, EventData);
            }
            catch (Exception)
            {

                throw new Exception("Can't set function data.");
            }
            
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
            InitiatedByFunctionName = callerName;
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
