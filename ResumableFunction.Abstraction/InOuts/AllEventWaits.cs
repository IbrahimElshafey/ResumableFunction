using System.Linq.Expressions;

namespace ResumableFunction.Abstraction.InOuts
{
    public class AllEventWaits : EventWaitingResult
    {
        public SingleEventWait[] WaitingEvents { get; set; }
        public SingleEventWait[] MatchedEvents { get; set; }
        public Expression<Func<int, bool>> WhenCountExpression { get; private set; }

        public AllEventWaits WhenCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }
    }
}