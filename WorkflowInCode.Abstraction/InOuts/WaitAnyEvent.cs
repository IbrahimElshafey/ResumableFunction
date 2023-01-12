namespace WorkflowInCode.Abstraction.InOuts
{
    public class WaitAnyEvent : EventWaiting
    {
        public EventWaiting[] Events { get; set; }
        public EventWaiting MatchedEvent { get; set; }
    }
}