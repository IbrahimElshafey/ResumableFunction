namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AnyEventWait : ManyWaits
    {
        public EventWait MatchedEvent { get; internal set; }

        internal void SetMatchedEvent(EventWait currentWait)
        {
            WaitingEvents.ForEach(wait=>wait.Status = WaitStatus.Skipped);
            MatchedEvent = currentWait;
            Status = WaitStatus.Completed;
        }
    }
}