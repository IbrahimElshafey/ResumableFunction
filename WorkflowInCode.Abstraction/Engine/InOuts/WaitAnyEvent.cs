namespace WorkflowInCode.Abstraction.Engine.InOuts
{
    public class WaitAnyEvent : WorkflowEvent
    {
        public WorkflowEvent[] Events { get; set; }
        public WorkflowEvent MatchedEvent { get; set; }
    }
}