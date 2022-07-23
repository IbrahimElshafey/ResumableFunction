namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    /// <summary>
    /// Event that created internaly in the engine when a workflow step calls `Workflow.PushInternalEvent`
    /// This events will not be handled by the events queue service but
    /// the running instance will serach for the steps that may be fired by this instance and will execute it
    /// </summary>
    /// <typeparam name="EventData"></typeparam>
    public class InternalEvent<EventData> : IEvent<EventData>
    {
        public Guid InstanceId { get; set; }
        public InternalEvent(string name) => this.Name = name;
        public string Name { get; private set;}

        public EventData Data { get; private set; }
    }
}
