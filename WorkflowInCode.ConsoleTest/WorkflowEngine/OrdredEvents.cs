namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public abstract class EventCollection
    {
        public EventCollection AddEventTrigger<EventData>(
           IEvent<EventData> eventTrigger,
           Func<EventData, bool> eventFilter)
        {
            return this;
        }
    }

    /// <summary>
    /// All events must be matched in the order they added to activate the step
    /// </summary>
    public class OrderedSequenceEvents: EventCollection
    {

    }

   
    /// <summary>
    /// Step will be activated based on custom logic
    /// </summary>
    public class CustomOrderEvents: EventCollection
    {

    }

    /// <summary>
    /// All events must be matched in any order to activate the step
    /// </summary>
    public class AllOfEvents : EventCollection
    {

    }

    /// <summary>
    /// Any event must be matched to activate the step
    /// </summary>
    public class AnyOneOfEvents : EventCollection
    {

    }
}
