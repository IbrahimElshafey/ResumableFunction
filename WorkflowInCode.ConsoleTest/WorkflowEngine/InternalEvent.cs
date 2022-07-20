namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    /// <summary>
    /// Event that created internaly in the engine
    /// </summary>
    /// <typeparam name="EventData"></typeparam>
    public class InternalEvent<EventData> : IEvent<EventData>
    {
        public InternalEvent(string name,EventData data)
        {
            this.Name = name;
            Data = data;
        }
        public string Name { get; private set;}

        public EventData Data { get; private set; }
    }
}
