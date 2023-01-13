namespace WorkflowInCode.Abstraction.InOuts
{
    public class AnyEventWaiting : EventWaitingResult
    {
        public SingleEventWaiting[] Events { get; set; }
        public SingleEventWaiting MatchedEvent { get; set; }
    }
}