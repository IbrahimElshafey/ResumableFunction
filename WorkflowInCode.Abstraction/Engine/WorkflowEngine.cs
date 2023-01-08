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
           //load and wait for the first event
           //create empty instance and set waiting list to the event expected
           //save state
        }

        public void EventReceived(ISubscribedEvent subscribedEvent)
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
