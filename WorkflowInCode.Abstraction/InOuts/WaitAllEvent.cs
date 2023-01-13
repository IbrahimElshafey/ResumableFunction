using System.Linq.Expressions;

namespace WorkflowInCode.Abstraction.InOuts
{
    public class AllEventWaiting : EventWaitingResult
    {
        public SingleEventWaiting[] WaitingEvents { get; set; }
        public SingleEventWaiting[] MatchedEvents { get; set; }
        public Expression<Func<int, bool>> WhenCountExpression { get; private set; }

        public AllEventWaiting WhenCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }
    }
}