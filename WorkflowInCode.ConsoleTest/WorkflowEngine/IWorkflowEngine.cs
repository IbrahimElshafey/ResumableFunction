using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public class WorkflowEngine : IWorkflowEngine
    {
        public Task End()
        {
            throw new NotImplementedException();
        }

        public Task ExpectNextStep(string stepName, string? tag = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterGlobalEventFilter(Func<dynamic, Task<bool>> globalFilterMethod)
        {
            throw new NotImplementedException();
        }

        public Task RegisterStep<EventData>(string stepName, IEvent<EventData> stepEvent, Func<EventData, Task> stepAction, Func<EventData, bool>? eventFilter = null, EventMarchingOption eventMatchingOption = EventMarchingOption.OneInstancePerEvent, Func<Task>? cancelAction = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveExpectations(string? tag = null)
        {
            throw new NotImplementedException();
        }

        public Task SaveState()
        {
            throw new NotImplementedException();
        }

        public Task UserTask<EventData>(string taskName, ICommand initiationCommand, IEvent<EventData> userActionEvent, Func<EventData, Task> afterUserAction)
        {
            throw new NotImplementedException();
        }
    }
    public interface IWorkflowEngine
    {
        

        /// <summary>
        /// Rigister a workflow step that executed when an event received
        /// </summary>
        /// <typeparam name="EventData"></typeparam>
        /// <param name="stepName">Unique name for the step in the workflow</param>
        /// <param name="stepEvent">Is the event that fire/trigger the step</param>
        /// <param name="stepAction">The code we execute after event fired</param>
        /// <param name="eventFilter">To find the right workflow instance that must be loaded(You must write this inside the step body)</param>
        /// <returns></returns>

        Task RegisterStep<EventData>(
            string stepName,
            IEvent<EventData> stepEvent,
            Func<EventData, Task> stepAction,
            Func<EventData, bool>? eventFilter = null,
            EventMarchingOption eventMatchingOption = EventMarchingOption.OneInstancePerEvent,
            Func<Task>? cancelAction =null);
       



        /// <summary>
        /// The engine will update the expected steps-events list for the active instance
        /// </summary>
        /// <param name="stepName">step name you defined when registration</param>
        /// <returns></returns>
        Task ExpectNextStep(string stepName,string? tag =null);
        
        /// <summary>
        /// When called the engine will clear the expected events list
        /// and for each event the engine will execute the CancelAction associated wit the steps that cancled
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task RemoveExpectations(string? tag =null);

        /// <summary>
        /// Must be called at least once when the workflow ended
        /// When this method called the engine will mark the instance as finished
        /// </summary>
        /// <returns></returns>
        Task End();



        /// <summary>
        /// Filter applied to any event received by the workflow
        /// </summary>
        void RegisterGlobalEventFilter(Func<dynamic, Task<bool>> globalFilterMethod);
        /// <summary>
        /// Task delegated to a user to take an action
        /// </summary>
        /// <typeparam name="EventData"></typeparam>
        /// <param name="taskName">task name</param>
        /// <param name="initiationCommand">send task to user command</param>
        /// <param name="userActionEvent">user action event that engine must wait to continue</param>
        /// <param name="afterUserAction">action done based on user action</param>
        /// <returns></returns>
        Task UserTask<EventData>(string taskName,
            ICommand initiationCommand,
            IEvent<EventData> userActionEvent,
            Func<EventData, Task> afterUserAction);
        Task SaveState();
    }

    public enum EventMarchingOption
    {
        OneInstancePerEvent,
        MultiplesInstancesPerEvent
    }
}
