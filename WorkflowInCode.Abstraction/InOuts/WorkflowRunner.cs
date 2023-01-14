namespace WorkflowInCode.Abstraction.InOuts
{
    public class WorkflowRunner
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public EventWaitingResult WaitingEvent { get; set; }
    }
}