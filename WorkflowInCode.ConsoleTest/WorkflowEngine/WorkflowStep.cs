using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public class WorkflowStep<EventData>
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
        public string StepName { get; set; }
        public string Path { get; set; }
        public IEvent<EventData> WakeUpEvent { get; set; }
        public Func<EventData, Task> StepAction { get; set; }
        //public Func<EventData, bool>? EventFilter { get; set; }
        //public EventMarchingOption EventMatchingOption { get; set; }
        public Func<Task>? CancelAction { get; set; }
    }
}
