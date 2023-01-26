namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AnyEventWait : Wait
    {
        public EventWait[] Events { get; set; }
        public EventWait MatchedEvent { get; set; }
    }
}