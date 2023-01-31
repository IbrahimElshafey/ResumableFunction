using System.Linq.Expressions;

namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllEventsWait : ManyWaits
    {
        public EventWait[] MatchedEvents { get; internal set; }
        public Expression<Func<int, bool>> WhenCountExpression { get; internal set; }

        public AllEventsWait WhenCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }
    }
}