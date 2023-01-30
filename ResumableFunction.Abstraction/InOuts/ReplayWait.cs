using System;
using System.Linq.Expressions;

namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class ReplayWait<EventData> : ReplayWait where EventData : class, IEventData, new()
    {
        public Type EventDataType { get; internal set; }
        public LambdaExpression MatchExpression { get; internal set; }
        internal ReplayWait(string name) : base(name)
        {
            IsSingle = true;
            EventDataType = typeof(EventData);
        }

        public ReplayWait<EventData> Match(Expression<Func<EventData, bool>> func)
        {
            MatchExpression = func;
            return this;
        }
    }
    public class ReplayWait : Wait
    {
        public string GotoWaitName { get; private set; }
        public bool IsSingle { get; internal set; }

        public string InitiatedByFunction { get; internal set; }

        internal ReplayWait(string name)
        {
            GotoWaitName = name;
        }


    }




}
