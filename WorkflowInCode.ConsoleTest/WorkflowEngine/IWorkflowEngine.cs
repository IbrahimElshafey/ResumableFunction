using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IWorkflowEngine
    {
        /// <summary>
        /// Workflow step that executed when an event received
        /// </summary>
        /// <typeparam name="EventData"></typeparam>
        /// <param name="stepName">Unique name for the step in the workflow</param>
        /// <param name="stepEvent">Is the event that fire the action we want to take</param>
        /// <param name="stepAction">The code we execute after event fired</param>
        /// <param name="eventFilter">To find the right workflow instance that must be loaded(You must write this inside the step body)</param>
        /// <returns></returns>
        Task RegisterStep<EventData>(
            string stepName,
            IEvent<EventData> stepEvent,
            Func<EventData, Task> stepAction,
            Func<EventData, bool>? eventFilter = null);

        Task<List<EventData>> GetStepEventsHistory<EventData>();
        /// <summary>
        /// [to be deleted] Workflow step that executed when an multiple events received
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stepTriggers">Multiple events that activate the step</param>
        /// <param name="eventsCollectorFunction">A method that collect events</param>
        /// <returns></returns>
        Task RegisterStep(string stepName, EventCollection stepTriggers, Func<object, Task> eventsCollectorFunction);

        /// <summary>
        /// [to be deleted] The engine will update the expected steps-events list for the active instance
        /// </summary>
        /// <param name="stepName">step name you defined when registration</param>
        /// <returns></returns>
        Task ExpectNextStep(string stepName);

        /// <summary>
        /// The engine will update the expected steps-events list for the active instance
        /// </summary>
        /// <param name="stepName">step name you defined when registration</param>
        /// <returns></returns>
        Task AddEventExpectation<EventData>(IEvent<EventData> expectedEvent);
        Task AddEventExpectation(string expectedEvent);


        /// <summary>
        /// Must be called at least once when the workflow ended
        /// When this method called the engine will mark the instance as finished
        /// </summary>
        /// <returns></returns>
        Task End();

        /// <summary>
        /// Create new events that used internally
        /// the engine will activate the workflow steps that can be triggred by workflow engine directly
        /// </summary>
        /// <typeparam name="EventData"></typeparam>
        /// <param name="internalEvent"></param>
        /// <returns></returns>
        Task PushInternalEvent<EventData>(InternalEvent<EventData> internalEvent);
        Task PushInternalEvent(string name,dynamic data=null);




        /// <summary>
        /// Filter applied to any event received by the workflow
        /// </summary>
        void RegisterGlobalEventFilter(Func<dynamic, Task<bool>> globalFilterMethod);

        Task SaveState();
    }

    public enum EventsExpectationMethod
    {
        Deterministic,
        NonDeterministic
    }
}
