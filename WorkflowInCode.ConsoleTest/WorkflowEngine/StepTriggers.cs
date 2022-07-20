namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public class StepTriggers
    {
        //Func<object, bool> EventsCollector
        public StepTriggers AddEventTrigger<EventData>(
            IEvent<EventData> eventTrigger,
            Func<EventData, bool> eventFilter)
        {
            return this;
        }

    }
}
