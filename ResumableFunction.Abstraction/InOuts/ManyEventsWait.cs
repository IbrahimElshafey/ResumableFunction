using System.Linq.Expressions;

namespace ResumableFunction.Abstraction.InOuts
{
    public class ManyEventsWait : Wait
    {
        public List<EventWait> WaitingEvents { get; internal set; } = new List<EventWait>();
        public LambdaExpression WhenCountExpression { get; internal set; }
        public EventWait MatchedEvent => WaitingEvents?.Single(x => x.Status == WaitStatus.Completed);

        public List<EventWait> MatchedEvents =>
            WaitingEvents?.Where(x => x.Status == WaitStatus.Completed).ToList();
       

        public ManyEventsWait WhenMatchedCount(Expression<Func<int, bool>> matchCountFilter)
        {
            WhenCountExpression = matchCountFilter;
            return this;
        }

        internal void MoveToMatched(EventWait currentWait)
        {
            var matchedEvent = WaitingEvents.First(x => x.Id == currentWait.Id);
            matchedEvent.Status = WaitStatus.Completed;
            CheckIsDone();
        }

        private bool CheckIsDone()
        {
            if (WhenCountExpression is null)
            {
                var required = WaitingEvents.Count(x => x.IsOptional == false);
                //MatchedEvents.Count include optional
                Status = required == MatchedEvents.Count ? WaitStatus.Completed : Status;
            }
            else
            {
                var matchedCount = MatchedEvents.Count;
                var matchCompiled = (Func<int, bool>)WhenCountExpression.Compile();
                Status = matchCompiled(matchedCount) ? WaitStatus.Completed : Status;
            }
            return Status == WaitStatus.Completed;
        }
        internal void SetMatchedEvent(EventWait currentWait)
        {
            WaitingEvents.ForEach(wait => wait.Status = WaitStatus.Skipped);
            var matchedEvent = WaitingEvents.First(x => x.Id == currentWait.Id);
            matchedEvent.Status = WaitStatus.Completed;
            Status = WaitStatus.Completed;
        }
    }
}