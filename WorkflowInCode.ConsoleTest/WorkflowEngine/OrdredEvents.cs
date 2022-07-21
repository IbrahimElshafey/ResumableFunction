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
    /// All events must be matched in the order they added
    /// </summary>
    public class OrderedSequenceEvents: EventCollection
    {

    }

   
    /// <summary>
    /// Next expected events handled in the code 
    /// </summary>
    public class CustomOrderEvents: EventCollection
    {

    }

    /// <summary>
    /// All events must be matched in the any order
    /// </summary>
    public class AllOfEvents : EventCollection
    {

    }

    /// <summary>
    /// Any event must be matched
    /// </summary>
    public class AnyOneOfEvents : EventCollection
    {

    }
}
