using System.Linq.Expressions;

namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllEventWait : Wait
    {
        public EventWait[] WaitingEvents { get; set; }
        public EventWait[] MatchedEvents { get; set; }
        public Expression<Func<int, bool>> WhenCountExpression { get; private set; }

        public AllEventWait WhenCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }
    }
}