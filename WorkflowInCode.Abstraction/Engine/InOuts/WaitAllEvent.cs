namespace WorkflowInCode.Abstraction.Engine.InOuts
{
    public class WaitAllEvent : WorkflowEvent
    {
        public int MustMatchCount { get; set; }
        public WorkflowEvent[] WaitingEvents { get; set; }
        public WorkflowEvent[] MatchedEvents { get; set; }
    }
}