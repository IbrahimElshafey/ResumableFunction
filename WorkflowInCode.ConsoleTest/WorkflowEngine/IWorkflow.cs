using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IWorkflow<WorkflowContextData>
    {
        Task Ended();
        Task ExpectNextStep<EventData>(Func<Task, EventData> nextStep);
        Task RegisterStep<EventData>(
            IEvent<EventData> stepEvent,
            Func<Task, EventData> stepAction,
            Func<WorkflowContextData, EventData, bool> eventFilter = null);
        Task RegisterStartStep<EventData>(
          Func<Task, EventData> stepAction,
          IEvent<EventData> stepEvent);

        //we should Unsubscribe timer events after the eventFilter method return true or calling it explict in workflow like
        //ITimerJobs.Unsubscribe("EventNameUsed");
        //eventFilter == event activation function when return true the step action executed
    }
}
