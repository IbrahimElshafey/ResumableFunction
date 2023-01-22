namespace ResumableFunction.Abstraction.InOuts
{
    public class AnyEventWait : EventWaitingResult
    {
        public SingleEventWait[] Events { get; set; }
        public SingleEventWait MatchedEvent { get; set; }
    }
}