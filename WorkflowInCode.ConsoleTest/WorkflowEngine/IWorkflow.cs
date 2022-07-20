using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public StepTriggers SetCollectorMethod(Func<object, bool> collectorFunction)
        {
            return this;
        }
    }
    public interface IWorkflow
    {
        Task End();
        Task ExpectNextStep<EventData>(Func<Task, EventData> nextStep, string arrowText = null);
        Task RegisterStep<EventData>(
            IEvent<EventData> stepEvent,
            Func<Task, EventData> stepAction,
            Func<EventData, bool> eventFilter = null);
        //Task RegisterStep(
        //   IEventsCollector<WorkflowContextData> stepEvent,
        //   Func<Task, EventData> stepAction);



        Task RegisterStep<T>(StepTriggers stepTriggers, Func<Task, T> stepAction);

        Task PushInternalEvent<EventData>(InternalEvent<EventData> internalEvent);

        //we should Unsubscribe timer events after the eventFilter method return true or
        //calling it explict in workflow like (this is the right solution)
        //ITimerJobs.Unsubscribe("EventNameUsed");

        //eventFilter == event activation function when return true the step action executed


    }
}
