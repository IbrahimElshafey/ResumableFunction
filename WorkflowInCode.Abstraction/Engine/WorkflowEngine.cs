using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.Abstraction.Engine
{
    public class WorkflowEngine
    {
        public void RegisterWorkflow<T>(WorkflowInstance<T> workflowInstance)
        {
           //initate instance and run workflow
           //wait for the first event
           //create empty instance and set waiting list to the event/s expected
           //save state
        }

        public void EventReceived(IEvent subscribedEvent)
        {
            //event comes to the engine
            //engine search workflow instances table to find if any workflow waits for this event type
            //engine load-related query for matched workflows instance and find matching function database query
            //search with query and find active instances
            //load context data and start/resume active instance workflow
            ActivateWorkflowInstance();
        }

        protected void ActivateWorkflowInstance()
        {

        }
    }
}
