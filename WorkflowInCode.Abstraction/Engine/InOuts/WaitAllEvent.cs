using System.Linq.Expressions;

namespace WorkflowInCode.Abstraction.Engine.InOuts
{
    public class WaitAllEvent : WorkflowEvent
    {
        public int MustMatchCount { get; set; }
        public WorkflowEvent[] WaitingEvents { get; set; }
        public WorkflowEvent[] MatchedEvents { get; set; }
        public Expression<Func<int, bool>> WhenCountExpression { get; private set; }

        public WorkflowEvent WhenCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }
    }
}