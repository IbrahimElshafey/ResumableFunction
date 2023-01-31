namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class ManyWaits : Wait
    {
        public EventWait[] WaitingEvents { get; internal set; }
    }
    public sealed class AnyEventWait : ManyWaits
    {
        public EventWait MatchedEvent { get; internal set; }
    }
}