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

      
        Task ExpectNextStep<EventData>(Func<EventData, Task> nextStep, string arrowText = null);
        
        /// <summary>
        /// Workflow step that executed when an event received
        /// </summary>
        /// <typeparam name="EventData"></typeparam>
        /// <param name="stepEvent">Is the event that fire the action we want to take</param>
        /// <param name="stepAction">The code we execute after event fired</param>
        /// <param name="eventFilter">To find the right workflow instance that must be loaded</param>
        /// <returns></returns>
        Task RegisterStep<EventData>(
            IEvent<EventData> stepEvent,
            Func<EventData, Task> stepAction,
            Func<EventData, bool> eventFilter = null);

        /// <summary>
        /// Workflow step that executed when an multiple events received
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stepTriggers">Multiple events that activate the step</param>
        /// <param name="eventsCollectorFunction">A method that collect events</param>
        /// <returns></returns>
        Task RegisterStep(EventCollection stepTriggers, Func<object, Task> eventsCollectorFunction);

        /// <summary>
        /// Create new events that used internally inside 
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
