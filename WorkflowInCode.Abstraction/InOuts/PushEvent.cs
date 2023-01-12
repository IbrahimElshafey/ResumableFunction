namespace WorkflowInCode.Abstraction.InOuts
{
    public class PushEvent
    {
        public string ProviderName { get; set; }
        public string EventType { get; set; }
        public object Payload { get; set; }
    }



}
