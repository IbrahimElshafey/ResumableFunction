using System.Linq.Expressions;

namespace WorkflowInCode.Abstraction.InOuts
{
    public class WaitAllEvent : EventWaiting
    {
        public int MustMatchCount { get; set; }
        public EventWaiting[] WaitingEvents { get; set; }
        public EventWaiting[] MatchedEvents { get; set; }
        public Expression<Func<int, bool>> WhenCountExpression { get; private set; }

        public EventWaiting WhenCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }
    }
}