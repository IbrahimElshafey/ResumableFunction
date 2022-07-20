using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IWorkflow<WorkflowContextData>
    {
        Task Ended(dynamic x=null);
        Task ExpectNextStep<EventData>(Func<Task, EventData> nextStep, string arrowText = null);
        Task RegisterStep<EventData>(
            IExternalEvent<EventData> stepEvent,
            Func<Task, EventData> stepAction,
            Func<WorkflowContextData, EventData, bool> eventFilter = null);

        Task RegisterStartStep<EventData>(
          Func<Task, EventData> stepAction,
          IExternalEvent<EventData> stepEvent,
          params NextExpectedStep[] expectedNextSteps);

        //we should Unsubscribe timer events after the eventFilter method return true or
        //calling it explict in workflow like (this is the right solution)
        //ITimerJobs.Unsubscribe("EventNameUsed");

        //eventFilter == event activation function when return true the step action executed


        Task RegisterStep<EventData>(
            IExternalEvent<EventData> stepEvent,
            Func<Task, EventData> stepAction,
            Func<WorkflowContextData, EventData, bool>? eventFilter = null,
            params NextExpectedStep[] expectedNextSteps);
    }
}
