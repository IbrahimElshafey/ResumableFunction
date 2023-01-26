using System.Linq.Expressions;

namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllEventsWait : Wait
    {
        public EventWait[] WaitingEvents { get; set; }
        public EventWait[] MatchedEvents { get; set; }
        public Expression<Func<int, bool>> WhenCountExpression { get; private set; }

        public AllEventsWait WhenCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }
    }
}