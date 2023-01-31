namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AnyEventWait : ManyWaits
    {
        public EventWait MatchedEvent { get; internal set; }
    }
}