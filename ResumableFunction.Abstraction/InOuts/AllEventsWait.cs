using System.Linq.Expressions;
using System.Security.Principal;

namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllEventsWait : ManyWaits
    {
        public List<EventWait> MatchedEvents { get; internal set; } = new List<EventWait>();
        public Expression<Func<int, bool>> WhenCountExpression { get; internal set; }

        public AllEventsWait WhenMatchedCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }

        internal void MoveToMatched(EventWait currentWait)
        {
            MatchedEvents.Add(currentWait);
            WaitingEvents.Remove(currentWait);
            CheckIsDone();
        }

        private bool CheckIsDone()
        {
            if(WhenCountExpression is null)
            {
                var required = WaitingEvents.Count(x => x.IsOptional == false);
                IsDone = required == 0;
            }
            else
            {
                var matchedCount = MatchedEvents.Count;
                var matchCompiled = WhenCountExpression.Compile();
                IsDone = matchCompiled(matchedCount);
            }
            return IsDone;
        }
    }
}