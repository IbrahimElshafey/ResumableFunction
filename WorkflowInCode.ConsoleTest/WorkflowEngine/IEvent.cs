using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IEvent<EventData>
    {
        /// <summary>
        /// The engine will execute this function when a workflow class activated
        /// If the workflow has a step with that event as a trigger
        /// </summary>
        /// <returns>Nothing or Error</returns>
        Task Subscribe();
        
        /// <summary>
        /// The engine will calls this method when the workflow deactivated 
        /// The workflow deactivated when engine admin mark it as inactive
        /// Or when new workflow version submitted and no instance is related to the old ones
        /// </summary>
        /// <returns></returns>
        Task Unsubscribe();


        Task<EventData> ReceivingEvent(Func<EventData, bool> filter,string stepName);
    }
}
