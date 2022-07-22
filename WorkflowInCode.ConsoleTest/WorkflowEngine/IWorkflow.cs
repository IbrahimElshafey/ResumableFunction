using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IWorkflow
    {
        /// <summary>
        /// Must be called at least once 
        /// </summary>
        /// <returns></returns>
        Task End();

      
        Task ExpectNextStep(string stepName);
        
        /// <summary>
        /// Workflow step that executed when an event received
        /// </summary>
        /// <typeparam name="EventData"></typeparam>
        /// <param name="stepEvent">Is the event that fire the action we want to take</param>
        /// <param name="stepAction">The code we execute after event fired</param>
        /// <param name="eventFilter">To find the right workflow instance that must be loaded(You must write this inside the step body)</param>
        /// <returns></returns>
        Task RegisterStep<EventData>(
            string stepName,
            IEvent<EventData> stepEvent,
            Func<EventData, Task> stepAction);

        /// <summary>
        /// Workflow step that executed when an multiple events received
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stepTriggers">Multiple events that activate the step</param>
        /// <param name="eventsCollectorFunction">A method that collect events</param>
        /// <returns></returns>
        Task RegisterStep(string stepName,EventCollection stepTriggers, Func<object, Task> eventsCollectorFunction);

        /// <summary>
        /// Create new events that used internally
        /// the engine will activate the workflow steps that can be triggred by workflow engine directly
        /// </summary>
        /// <typeparam name="EventData"></typeparam>
        /// <param name="internalEvent"></param>
        /// <returns></returns>
        Task PushInternalEvent<EventData>(InternalEvent<EventData> internalEvent);

        //we should Unsubscribe timer events after the eventFilter method return true or
        //calling it explict in workflow like (this is the right solution)
        //ITimerJobs.Unsubscribe("EventNameUsed");

        //eventFilter == event activation function when return true the step action executed


    }
}
